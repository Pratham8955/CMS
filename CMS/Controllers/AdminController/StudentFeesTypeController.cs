using CMS.DTOs.StudentFeesType;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentFeesTypeController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public StudentFeesTypeController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("addStudentFeeType")]
        public async Task<IActionResult> AddStudentFeeType([FromBody] StudentFeesTypeDTO dto)
        {

            var feeStructure = await _context.FeeStructures
                                              .FirstOrDefaultAsync(fs => fs.FeeStructureId == dto.FeeStructureId);

            if (feeStructure == null)
            {
                return BadRequest(new { success = false, message = "Fee Structure not found." });
            }

            decimal totalAmount = feeStructure.DefaultAmount;

            decimal tuitionFees = totalAmount * 0.50m;
            decimal labFees = totalAmount * 0.20m;
            decimal collegeGroundFee = totalAmount * 0.15m;
            decimal internalExam = totalAmount * 0.15m;


            if (tuitionFees + labFees + collegeGroundFee + internalExam != totalAmount)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Calculated fees do not sum up to the Default Amount."
                });
            }

            var feeType = new StudentFeesType
            {
                FeeStructureId = dto.FeeStructureId,
                TuitionFees = tuitionFees,
                LabFees = labFees,
                CollegeGroundFee = collegeGroundFee,
                InternalExam = internalExam
            };

            _context.StudentFeesTypes.Add(feeType);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Student Fee Type added successfully.",
                FeeType = new
                {
                    feeType.FeetypeId,
                    feeType.FeeStructureId,
                    feeType.TuitionFees,
                    feeType.LabFees,
                    feeType.CollegeGroundFee,
                    feeType.InternalExam
                }
            });
        }


        // GET ALL
        [HttpGet("getAllStudentFees")]
        public async Task<IActionResult> getAllStudentFees()
        {
            var studentFees = await _context.StudentFeesTypes.Select(s => new StudentFeesTypeDTO
            {
                FeetypeId = s.FeetypeId,
                FeeStructureId = s.FeeStructureId,
                TuitionFees = s.TuitionFees,
                LabFees = s.LabFees,
                CollegeGroundFee = s.CollegeGroundFee,
                InternalExam = s.InternalExam
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Fee Type Fetched Successfully",
                StudentFees = studentFees
            });
        }

        // GET BY ID
        [HttpGet("getByIdStudentFees/{id}")]
        public async Task<IActionResult> GetByIdStudentFees(int id)
        {
            var feeType = await _context.StudentFeesTypes.FindAsync(id);
            if (feeType == null)
                return NotFound(new { success = false, message = "Fee Type not found." });

            return Ok(new { success = true, data = feeType });
        }

        // UPDATE
        [HttpPut("updateStudentFees/{id}")]
        public async Task<IActionResult> UpdateStudentFees(int id, [FromBody] StudentFeesTypeDTO dto)
        {
            if (_context.StudentFeesTypes.Any(fs => fs.FeeStructureId == dto.FeeStructureId && fs.FeetypeId != id))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Fee Structure for this department and semester already exists."
                });
            }

            var feeType = await _context.StudentFeesTypes.FindAsync(id);
            if (feeType == null)
                return NotFound(new { success = false, message = "Fee Type not found." });

            feeType.TuitionFees = dto.TuitionFees;
            feeType.LabFees = dto.LabFees;
            feeType.CollegeGroundFee = dto.CollegeGroundFee;
            feeType.InternalExam = dto.InternalExam;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Fee Type updated successfully.", data = feeType });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Student Fees Type: {ex.Message}");
            }



        }

        // DELETE
        [HttpDelete("deleteStudentFees/{id}")]
        public async Task<IActionResult> DeleteStudentFees(int id)
        {
            var feeType = await _context.StudentFeesTypes.FindAsync(id);
            if (feeType == null)
                return NotFound(new { success = false, message = "Fee Type not found." });

            _context.StudentFeesTypes.Remove(feeType);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Fee Type deleted successfully." });
        }
    }



}
