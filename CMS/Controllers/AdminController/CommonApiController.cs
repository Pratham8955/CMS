using System.Net.Mail;
using System.Net;
using CMS.DTOs.SemesterDTO;
using CMS.DTOs.StudentDTO;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.DTOs.GroupMasterDTO;

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

            var oldPasswordHash = data.Password;

            if (BCrypt.Net.BCrypt.Verify(forgetPassStudent.Password, oldPasswordHash))
            {
                return BadRequest(new { success = false, message = "New password cannot be same as old password." });
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

            var oldPasswordHash = data.Password;

            if (BCrypt.Net.BCrypt.Verify(forgetPassFaculty.Password, oldPasswordHash))
            {
                return BadRequest(new { success = false, message = "New password cannot be same as old password." });
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

        [HttpPost("send-otp")]
        public async Task<ActionResult> SendOtp([FromBody] SendOtpDTO sendOtpDto)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("otp", otp);
            HttpContext.Session.SetString("otpEmail", sendOtpDto.Email);

            bool emailSent = await SendEmail(sendOtpDto.Email, otp);

            if (!emailSent)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "failed to send OTP."
                });
            }
            else
            {
                return Ok(new
                {
                    success = true,
                    message = "OTP sent Successfully."
                });
            }
        }

        // email sending method
        private async Task<bool> SendEmail(string email, string otp)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ForgetPassword.html");
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
                    Subject = "Your OTP code for Reset Password.",
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


        // verify otp functionality
        [HttpPost("verify-otp")]
        public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var sessionOtp = HttpContext.Session.GetString("otp");
            var sessionEmail = HttpContext.Session.GetString("otpEmail");

            if (sessionOtp == null || sessionEmail == null || sessionOtp != verifyOtpDto.Otp || sessionEmail != verifyOtpDto.Email)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid Or Expired OTP."
                });
            }

            HttpContext.Session.Remove("otp");
            HttpContext.Session.Remove("otpEmail");

            return Ok(new
            {
                success = true,
                message = "OTP Verified Successfully."
            });
        }

        [HttpGet("departmentCounts/{depid}")]
        public async Task<IActionResult> GetDepartmentCounts(int depid)
        {
            var dep = await _context.Departments.FindAsync(depid);
            if (dep == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Department not found."
                });
            }

            var countFaculty = await _context.Faculties
                .Where(f => f.DeptId == depid)
                .CountAsync();

            var studentIds = await _context.Students
                .Where(s => s.DeptId == depid)
                .Select(s => s.StudentId)
                .ToListAsync();

            var countStudent = studentIds.Count;

            // FIXED: Use AsEnumerable to avoid WITH clause syntax error
            var paidStudentIds = _context.StudentFees
                .AsEnumerable()
                .Where(f => studentIds.Contains(f.StudentId) && f.Status == "Paid")
                .Select(f => f.StudentId)
                .Distinct()
                .ToList();

            var paidCount = paidStudentIds.Count;
            var unpaidCount = countStudent - paidCount;

            return Ok(new
            {
                success = true,
                message = "Counts fetched successfully.",
                CountFaculty = countFaculty,
                CountStudent = countStudent,
                PaidCount = paidCount,
                UnpaidCount = unpaidCount
            });
        }


    }
}
