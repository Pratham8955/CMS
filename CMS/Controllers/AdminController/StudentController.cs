using CMS.DTOs.StudentDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

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


        [HttpPut("updateStudents/{id}")]
        public async Task<IActionResult> updateStudents(int id, [FromBody] UpdateStudentDTO updatestudent)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"No student of this {id} is found");
            }
            student.StudentName = updatestudent.StudentName;
            student.Email = updatestudent.Email;
            student.Dob = updatestudent.Dob;
            student.Gender = updatestudent.Gender;
            student.Address = updatestudent.Address;
            student.City = updatestudent.City;
            student.State = updatestudent.State;
            student.Phone = updatestudent.Phone;
            student.DeptId = updatestudent.DeptId;
            student.CurrentSemester = updatestudent.CurrentSemester;

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

        [HttpDelete("deleteStudent/{id}")]
        public async Task<IActionResult> deleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            try
            {
                _context.Students.Remove(student);
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
