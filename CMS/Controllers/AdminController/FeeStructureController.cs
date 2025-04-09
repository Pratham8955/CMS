using CMS.DTOs.FeeStructureDTO;
using CMS.Models;
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
        public async Task<IActionResult> getFeeStructure()
        {
            var feeStruct = await _context.FeeStructures.Select(fs => new FeeStructureDTO
            {
                DeptId = fs.DeptId,
                SemId = fs.SemId,
                DefaultAmount = fs.DefaultAmount,
            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Fee Structure Fetch Succesfully",
                FeeStruct = feeStruct,
            });
        }

        [HttpPost("FeeStructure")]
        public async Task<IActionResult> FeeStructure(FeeStructureDTO dTO)
        {
            if (_context.FeeStructures.Any(fs => fs.DeptId == dTO.DeptId && fs.SemId == dTO.SemId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Fee Structure for this department and semester already exists."
                });
            }

            var feeStruct = new FeeStructure
            {
                DeptId = dTO.DeptId,
                SemId = dTO.SemId,
                DefaultAmount = dTO.DefaultAmount
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
                    feeStruct.DefaultAmount
                }
            });
        }

        [HttpPost("updateFeeStructure/{id}")]
        public async Task<IActionResult> updateFeeStructure(int id, [FromBody] FeeStructureDTO dTO)
        {
            if (_context.FeeStructures.Any(fs => fs.DeptId == dTO.DeptId && fs.SemId == dTO.SemId))
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
