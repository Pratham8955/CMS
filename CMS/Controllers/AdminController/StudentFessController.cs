using CMS.DTOs.StudentFeesDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentFessController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;
        public StudentFessController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("StudentFees")]
        public async Task<IActionResult> PayFees(StudentFessDTO dto)
        {
            var student = await _context.Students
                              .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);
            if (student == null)
                return BadRequest(new { success = false, message = "Student not found." });

            var feeStructure = await _context.FeeStructures.FindAsync(dto.FeeStructureId);
            if (feeStructure == null)
                return BadRequest(new { success = false, message = "Fee structure not found." });


            var payment = new StudentFee
            {
                StudentId = dto.StudentId,
                FeeStructureId = dto.FeeStructureId,
                PaidAmount = dto.PaidAmount,
                TotalAmount = dto.TotalAmount,
                Status = "Paid",
                TransactionId = dto.TransactionId,
                PaymentDate = DateTime.UtcNow
            };

            _context.StudentFees.Add(payment);
            await _context.SaveChangesAsync();


            var feeTypes = await _context.StudentFeesTypes
                                .Where(sft => sft.StudentId == null && sft.FeeStructureId == dto.FeeStructureId)
                                .ToListAsync();

            if (feeTypes.Count == 0)
            {
                return NotFound(new { success = false, message = "No matching fee types found." });
            }

            foreach (var feeType in feeTypes)
            {

                feeType.FeeId = payment.FeeId;  // Update FeeId to link to the payment
                feeType.TransactionDate = payment.PaymentDate;  // Set the TransactionDate for the payment
                feeType.StudentId = dto.StudentId;

            }

            await _context.SaveChangesAsync();

            //var pdfContent = await GeneratePdfFromFeeTypes(feeTypes);

            return Ok(new
            {
                success = true,
                message = "Payment successful.",
                data = new
                {
                    payment.FeeId,
                    payment.StudentId,
                    payment.TotalAmount,
                    payment.PaidAmount,
                    payment.Status,
                    payment.TransactionId,
                    payment.PaymentDate
                }
            });
        }


        [HttpGet("getFeeAndPaymentDetails/{studentId}")]
        public async Task<IActionResult> GetFeeAndPaymentDetails(int studentId)
        {
            // Fetch fee type details
            var feeType = await _context.StudentFeesTypes
                .Where(sft => sft.StudentId == studentId)
                .Select(sft => new
                {
                    sft.FeetypeId,
                    sft.FeeStructureId,
                    sft.TuitionFees,
                    sft.LabFees,
                    sft.CollegeGroundFee,
                    sft.InternalExam,
                    sft.TransactionDate
                }).FirstOrDefaultAsync();

            if (feeType == null)
            {
                return NotFound(new { success = false, message = "Fee type not found." });
            }

            // Fetch payment details
            var paymentDetails = await _context.StudentFees
                .Where(sf => sf.StudentId == studentId)
                .Select(sf => new
                {
                    sf.FeeId,
                    sf.PaidAmount,
                    sf.Status,
                    sf.TransactionId,
                }).FirstOrDefaultAsync();

            if (paymentDetails == null)
            {
                return NotFound(new { success = false, message = "Payment details not found." });
            }

            return Ok(new
            {
                success = true,
                feeType,
                paymentDetails
            });
        }













        [HttpGet("all-payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.StudentFees.Include(p => p.StudentFeesTypes).ToListAsync();
            return Ok(new { success = true, data = payments });
        }

        [HttpDelete("DeleteFees/{id}")]
        public async Task<IActionResult> DeleteFees(int id)
        {
            var tblpayment = await _context.StudentFees.FindAsync(id);
            if (tblpayment == null)
            {
                return NotFound();
            }

            _context.StudentFees.Remove(tblpayment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentFee>> GetFeesById(int id)
        {
            var tblpayment = await _context.StudentFees.FindAsync(id);

            if (tblpayment == null)
            {
                return NotFound();
            }

            return tblpayment;
        }
    }
}
