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
            var students = await _context.Students.Select(dto => new StudentDTOFORIMG
            {
                StudentId=dto.StudentId,
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
                StudentImg=dto.StudentImg,
                TenthPassingYear = dto.TenthPassingYear,
                TenthPercentage=dto.TenthPercentage,
                TenthSchool=dto.TenthSchool,
                Tenthmarksheet=dto.Tenthmarksheet,
                TwelfthMarksheet=dto.TwelfthMarksheet,
                TwelfthPassingYear=dto.TwelfthPassingYear,
                TwelfthPercentage=dto.TwelfthPercentage,
                TwelfthSchool=dto.TwelfthSchool,
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
                StudentId = dto.StudentId

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

            try
            {
                // --- Handle Student Image ---
                if (updatestudent.StudentImg != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(student.StudentImg))
                    {
                        string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "studentProfile", student.StudentImg);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "studentProfile");
                    Directory.CreateDirectory(uploadsFolder);

                    string newImageName = Guid.NewGuid().ToString() + Path.GetExtension(updatestudent.StudentImg.FileName);
                    string newFilePath = Path.Combine(uploadsFolder, newImageName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await updatestudent.StudentImg.CopyToAsync(fileStream);
                    }

                    student.StudentImg = newImageName;
                }

                // --- Handle Tenth Marksheet PDF ---
                if (updatestudent.TenthMarksheet != null)
                {
                    // Delete old tenth marksheet if exists
                    if (!string.IsNullOrEmpty(student.Tenthmarksheet))
                    {
                        string oldTenthPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets", student.Tenthmarksheet);
                        if (System.IO.File.Exists(oldTenthPath))
                        {
                            System.IO.File.Delete(oldTenthPath);
                        }
                    }

                    // Save new tenth marksheet using original filename (no rename)
                    string tenthFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets");
                    Directory.CreateDirectory(tenthFolder);

                    string tenthFilePath = Path.Combine(tenthFolder, updatestudent.TenthMarksheet.FileName);

                    using (var stream = new FileStream(tenthFilePath, FileMode.Create))
                    {
                        await updatestudent.TenthMarksheet.CopyToAsync(stream);
                    }

                    student.Tenthmarksheet = updatestudent.TenthMarksheet.FileName;
                }

                // --- Handle Twelfth Marksheet PDF ---
                if (updatestudent.TwelfthMarksheet != null)
                {
                    // Delete old twelfth marksheet if exists
                    if (!string.IsNullOrEmpty(student.TwelfthMarksheet))
                    {
                        string oldTwelfthPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets", student.TwelfthMarksheet);
                        if (System.IO.File.Exists(oldTwelfthPath))
                        {
                            System.IO.File.Delete(oldTwelfthPath);
                        }
                    }

                    // Save new twelfth marksheet using original filename (no rename)
                    string twelfthFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets");
                    Directory.CreateDirectory(twelfthFolder);

                    string twelfthFilePath = Path.Combine(twelfthFolder, updatestudent.TwelfthMarksheet.FileName);

                    using (var stream = new FileStream(twelfthFilePath, FileMode.Create))
                    {
                        await updatestudent.TwelfthMarksheet.CopyToAsync(stream);
                    }

                    student.TwelfthMarksheet = updatestudent.TwelfthMarksheet.FileName;
                }

                // --- Update other student fields ---
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
                student.TenthPassingYear = updatestudent.TenthPassingYear;
                student.TenthPercentage = updatestudent.TenthPercentage;
                student.TenthSchool = updatestudent.TenthSchool;
                student.TwelfthPassingYear = updatestudent.TwelfthPassingYear;
                student.TwelfthPercentage = updatestudent.TwelfthPercentage;
                student.TwelfthSchool = updatestudent.TwelfthSchool;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating student: {ex.InnerException}");
            }
        }



        [HttpDelete("deleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound(new { success = false, message = "Student not found." });

                // Delete Student Image
                if (!string.IsNullOrEmpty(student.StudentImg))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "studentProfile", student.StudentImg);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                // Delete 10th Marksheet
                if (!string.IsNullOrEmpty(student.Tenthmarksheet))
                {
                    string tenthPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets", student.Tenthmarksheet);
                    if (System.IO.File.Exists(tenthPath))
                        System.IO.File.Delete(tenthPath);
                }

                // Delete 12th Marksheet
                if (!string.IsNullOrEmpty(student.TwelfthMarksheet))
                {
                    string twelfthPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students", "marksheets", student.TwelfthMarksheet);
                    if (System.IO.File.Exists(twelfthPath))
                        System.IO.File.Delete(twelfthPath);
                }

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Student deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
