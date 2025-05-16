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

            // Check if a payment already exists for this student and fee structure
            var existingPayment = await _context.StudentFees
                                                .FirstOrDefaultAsync(p => p.StudentId == dto.StudentId && p.FeeStructureId == dto.FeeStructureId && p.Status == "Paid");
            if (existingPayment != null)
            {
                return BadRequest(new { success = false, message = "Payment for this fee structure has already been made." });
            }

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





        [HttpGet("all-payments")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.StudentFees.Select(sub => new
            {
                sub.FeeId,
                sub.StudentId,
                sub.FeeStructureId,
                sub.PaidAmount,
                sub.TotalAmount,
                sub.Status,
                sub.TransactionId,
                sub.PaymentDate
            }).ToListAsync();
            return Ok(new { success = true, data = payments });
        }

        [HttpGet("allpaymentsByDep/{id}")]
        public async Task<IActionResult> GetAllPaymentsByDep(int id)
        {
            var dep = await _context.Departments.FindAsync(id);
            var depid = dep.DeptId;
            if (depid == null)
            {
                return NotFound();
            }

            var payments = await _context.Students.Where(pay => pay.DeptId == depid).Select(student => new
            {
                student.StudentId,
                student.StudentName,
                Fees = _context.StudentFees
                .Where(fee => fee.StudentId == student.StudentId)
                .Select(fee => new
                {
                    fee.FeeId,
                    fee.FeeStructureId,
                    fee.PaidAmount,
                    fee.TotalAmount,
                    fee.Status,
                    fee.TransactionId,
                    fee.PaymentDate
                })
                .FirstOrDefault()
            }).ToListAsync();
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

        [HttpGet("GetStudentFee/{studentId}")]
        public async Task<IActionResult> GetStudentFee(int studentId)
        {
            var fee = await _context.StudentFees
                .Where(f => f.StudentId == studentId)
                .Select(f => new
                {
                    f.FeeId,
                    f.StudentId,
                    f.FeeStructureId,
                    f.TotalAmount,
                    f.PaidAmount,
                    f.Status,
                    f.TransactionId,
                    f.PaymentDate,
                    DepartmentName = f.FeeStructure.Dept.DeptName,
                    SemesterName = f.FeeStructure.Sem.SemName,
                    Student_Name = f.Student.StudentName,
                    FeeType = _context.StudentFeesTypes
                                .Where(ft => ft.FeeStructureId == f.FeeStructureId)
                                .Select(ft => new
                                {
                                    ft.TuitionFees,
                                    ft.LabFees,
                                    ft.CollegeGroundFee,
                                    ft.InternalExam
                                })
                                .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (fee == null)
                return NotFound(new { success = false, message = "No fee record found for this student." });

            return Ok(fee);
        }



        [HttpGet("CheckPaymentStatus/{studentId}/{feeStructureId}")]
        public async Task<IActionResult> CheckPaymentStatus(int studentId, int feeStructureId)
        {
            var hasPaid = await _context.StudentFees
                .AnyAsync(p => p.StudentId == studentId && p.FeeStructureId == feeStructureId && p.Status == "Paid");

            return Ok(new
            {
                success = true,
                isPaid = hasPaid
            });
        }

    }
}
