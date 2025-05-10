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



        [HttpGet("getStudentsById/{id}")]

        public async Task<IActionResult> getStudentsById(int id)
        {
            var students = await _context.Students.Where(s => s.StudentId == id).Select(dto => new StudentDTOFORIMG
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
                StudentImg = dto.StudentImg,
                StudentId=dto.StudentId

            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Students Fetch Succesfully",
                student = students
            });
        }


        [HttpPut("updateStudents/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromForm] UpdateStudentDTO updatestudent)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound($"No student with id {id} found.");
            }

            // Check if a new image is provided
            if (updatestudent.StudentImg != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(student.StudentImg))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", student.StudentImg);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { success = false, message = $"Failed to delete old image: {ex.Message}" });
                        }
                    }
                }

                // Save the new image
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string newImageName = Guid.NewGuid().ToString() + Path.GetExtension(updatestudent.StudentImg.FileName);
                string newFilePath = Path.Combine(uploadsFolder, newImageName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await updatestudent.StudentImg.CopyToAsync(fileStream);
                }

                student.StudentImg = newImageName; // Update the StudentImg field
            }

            // Update the rest of the student fields
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
                return NoContent(); // No content as a response when update is successful
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating student: {ex.Message}");
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
