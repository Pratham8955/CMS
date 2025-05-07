using CMS.DTOs.FeeStructureDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeeStructureController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public FeeStructureController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("getFeeStructure")]
        public async Task<IActionResult> GetFeeStructure()
        {
            var feeStruct = await _context.FeeStructures.Select(fs => new FeeStructureDTO
            {
                DeptId = fs.DeptId,
                SemId = fs.SemId,
                DefaultAmount = fs.DefaultAmount,
                FeeStructureDescription=fs.FeeStructureDescription
                
            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Fee Structure Fetched Successfully",
                FeeStruct = feeStruct,
            });
        }


        [HttpPost("addFeeStructure")]
        public async Task<IActionResult> AddFeeStructure(FeeStructureDTO dTO)
        {
            // Check if the FeeStructure for the given DeptId and SemId already exists
            if (_context.FeeStructures.Any(fs => fs.DeptId == dTO.DeptId && fs.SemId == dTO.SemId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Fee Structure for this department and semester already exists."
                });
            }

            // Fetch Department Name and Semester Name using DeptId and SemId
            var department = await _context.Departments
                                           .FirstOrDefaultAsync(d => d.DeptId == dTO.DeptId);

            var semester = await _context.Semesters
                                         .FirstOrDefaultAsync(s => s.SemId == dTO.SemId);

            // If either department or semester is not found, return a bad request
            if (department == null || semester == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Department or Semester not found."
                });
            }

            string description = $"{department.DeptName} - {semester.SemName} ({dTO.DefaultAmount} Rs)";

            var feeStruct = new FeeStructure
            {
                DeptId = dTO.DeptId,
                SemId = dTO.SemId,
                DefaultAmount = dTO.DefaultAmount,
                FeeStructureDescription = description 
            };

            _context.FeeStructures.Add(feeStruct);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Fee Structure Added Successfully",
                FeeStruct = new
                {
                    feeStruct.DeptId,
                    feeStruct.SemId,
                    feeStruct.DefaultAmount,
                    feeStruct.FeeStructureDescription // Returning the description for reference
                }
            });
        }




        [HttpPost("updateFeeStructure/{id}")]
        public async Task<IActionResult> updateFeeStructure(int id, [FromBody] FeeStructureDTO dTO)
        {
            if (_context.FeeStructures.Any(fs => fs.FeeStructureId != id && fs.DeptId == dTO.DeptId && fs.SemId == dTO.SemId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Fee Structure for this department and semester already exists."
                });
            }

            var feeStruct = await _context.FeeStructures.FindAsync(id);
            if (feeStruct == null)
            {
                return NotFound($"No student of this {id} is found");
            }

            feeStruct.DeptId = dTO.DeptId;
            feeStruct.SemId = dTO.SemId;
            feeStruct.DefaultAmount = dTO.DefaultAmount;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Faculty: {ex.Message}");
            }
        }

        [HttpDelete("deleteFeeStructure/{id}")]
        public async Task<IActionResult> deleteFeeStructure(int id)
        {
            var feeStruct = await _context.FeeStructures.FindAsync(id);
            try
            {
                _context.FeeStructures.Remove(feeStruct);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Faculty: {ex.Message}");
            }

        }
    }
}
