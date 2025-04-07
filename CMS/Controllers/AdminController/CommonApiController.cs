using CMS.DTOs.SemesterDTO;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonApiController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public CommonApiController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetSemester")]

        public async Task<IActionResult> GetSemester()
        {
            var semester = await _context.Semesters.Select(s => new SemesterDTO
            {
                SemId = s.SemId,
                SemName = s.SemName,

            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Semesters fetch successfully.",
                Semester = semester,
            });
        }

        //get faculty according to department
        [HttpGet("GetFacultyByDepartment/{deptId}")]
        public async Task<IActionResult> GetFacultyByDepartment(int deptId)
        {
            var faculty = await _context.Faculties
                .Where(f => f.DeptId == deptId)
                .Select(f => new
                {
                    f.FacultyId,
                    f.FacultyName,
                    f.Email
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Faculty fetched successfully.",
                Faculty = faculty
            });
        }
    }
}
