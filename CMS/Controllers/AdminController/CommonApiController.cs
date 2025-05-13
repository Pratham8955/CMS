using System.Net.Mail;
using System.Net;
using CMS.DTOs.SemesterDTO;
using CMS.DTOs.StudentDTO;
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

        [HttpPost("forgetPasswordStudent")]
        public async Task<IActionResult> ForgetPasswordStudent(ForgetPassStudent forgetPassStudent)
        {
            var data = await _context.Students.FirstOrDefaultAsync(fp => fp.Email == forgetPassStudent.Email);
            if (data == null)
            {
                return NotFound(new { success = false, message = "Student not found." });
            }
            try
            {
                data.Password = BCrypt.Net.BCrypt.HashPassword(forgetPassStudent.Password);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in student: {ex.InnerException}");
            }
        }



        [HttpPost("forgetPasswordFaculty")]
        public async Task<IActionResult> forgetPasswordFaculty(ForgetPassStudent forgetPassFaculty)
        {
            var data = await _context.Faculties.FirstOrDefaultAsync(fp => fp.Email == forgetPassFaculty.Email);
            if (data == null)
            {
                return NotFound(new { success = false, message = "Faculty not found." });
            }
            try
            {
                data.Password = BCrypt.Net.BCrypt.HashPassword(forgetPassFaculty.Password);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error In Faculties: {ex.InnerException}");
            }
        }

        private async Task<bool> SendEmail(string email, string otp)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "OtpTemplate.html");
                string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                emailBody = emailBody.Replace("{{OTP}}", otp);

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("salipratham033@gmail.com", "zlbd txsz xmso uswe"),
                    EnableSsl = true,
                };

                var mailMsg = new MailMessage
                {
                    From = new MailAddress("salipratham033@gmail.com"),
                    Subject = "Your OTP code for Registration.",
                    Body = emailBody,
                    IsBodyHtml = true,

                };

                mailMsg.To.Add(email);
                await smtp.SendMailAsync(mailMsg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
