using CMS.DTOs.StudentDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;
        public StudentController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpGet("GetStudents")]

        public async Task<IActionResult> getStudents()
        {
            var students = await _context.Students.Select(dto => new StudentDTO
            {
                StudentName = dto.StudentName,
                Email = dto.Email,
                Dob = dto.Dob,
                Gender = dto.Gender,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Phone = dto.Phone,
                DeptId = dto.DeptId,
                CurrentSemester = dto.CurrentSemester,
            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Students Fetch Succesfully",
                student = students
            });
        }

    }
}
