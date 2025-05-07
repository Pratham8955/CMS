using CMS.DTOs.DepartmentDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public DepartmentController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //-------------------------------------
        //DepartmentsApi
        [HttpGet("GetDepartment")]
        public async Task<IActionResult> GetDepartment()
        {
            var department = await _context.Departments.Where(d => d.DeptName != "Admin").Select(s => new DepartmentsDTO
            {
                DeptId = s.DeptId,
                DeptName = s.DeptName,
                HeadOfDept = s.HeadOfDept,
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Departments fetch successfully.",
                Department = department,
            });
        }

        [HttpPost("AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentsDTO addDepartment)
        {
            try
            {
                if (_context.Departments.Any(fs => fs.DeptName == addDepartment.DeptName ))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "This department already exists."
                    });
                }

                var department = new Department
                {
                    DeptName = addDepartment.DeptName
                };
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Department Added Successfully",
                    Department = new
                    {
                        department.DeptName,
                        department.HeadOfDept,
                    },
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpPost("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentsDTO updateDepartment)
        {
            if (_context.Departments.Any(fs => fs.DeptName == updateDepartment.DeptName && fs.DeptId != id))

            {
                return BadRequest(new
                {
                    success = false,
                    message = "This department already exists."
                });
            }
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound($"Departments with Id {id} Not Found");
            }
            department.DeptName = updateDepartment.DeptName;
            department.HeadOfDept = updateDepartment.HeadOfDept;
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Department updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Department: {ex.Message}");
            }
        }

        [HttpDelete("DeleteDepartment")]

        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound($"Departments with Id {id} Not Found");
            }
            try
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Department deleted successfully." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Department: {ex.Message}");
            }
        }


    }
}
