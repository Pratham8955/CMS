using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Models;
using Microsoft.CodeAnalysis.Scripting;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CMS.DTOs.GroupMasterDTO;
using CMS.DTOs.StudentDTO;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public RegisterController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("fetchroles")]
        public async Task<ActionResult> FetchRoles()
        {
            var roles = await _context.GroupMasters.Select(r =>
            new GroupMasterDTO { GroupId = r.GroupId, Role = r.Role }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "roles fetch successfully.",
                Roles = roles,
            });
        }


        private string GenerateSecurePassword(int length = 10)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";

            var random = new Random();
            var chars = new List<char>
    {
        upper[random.Next(upper.Length)],
        special[random.Next(special.Length)],
        digits[random.Next(digits.Length)],
        lower[random.Next(lower.Length)],
    };

            string allChars = lower + upper + digits + special;
            for (int i = chars.Count; i < length; i++)
            {
                chars.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(chars.OrderBy(x => random.Next()).ToArray());
        }


        private async Task<bool> SendStudentEmail(string toEmail, string StudentName, string plainPassword)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "StudentEmailTemplate.html");
                string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                emailBody = emailBody
                    .Replace("{{StudentName}}", StudentName)
                    .Replace("{{Email}}", toEmail)
                    .Replace("{{Password}}", plainPassword);

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("salipratham033@gmail.com", "zlbd txsz xmso uswe"),
                    EnableSsl = true,
                };

                var mailMsg = new MailMessage
                {
                    From = new MailAddress("salipratham033@gmail.com"),
                    Subject = "Your Student Login Credentials",
                    Body = emailBody,
                    IsBodyHtml = true,
                };

                mailMsg.To.Add(toEmail);
                await smtp.SendMailAsync(mailMsg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromForm] StudentDTO dto)
        {
            try
            {
                if (_context.Students.Any(u => u.Email == dto.Email))
                    throw new Exception("Email Already Exists.");

                string? imageName = null;

           
                if (dto.StudentImg != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "students");
                    Directory.CreateDirectory(uploadsFolder); 

                    imageName = Guid.NewGuid().ToString() + Path.GetExtension(dto.StudentImg.FileName);
                    string filePath = Path.Combine(uploadsFolder, imageName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.StudentImg.CopyToAsync(fileStream);
                    }
                }

                string generatedPassword = GenerateSecurePassword();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);
                var student = new Student
                {
                    StudentName = dto.StudentName,
                    Email = dto.Email,
                    Password = hashedPassword,
                    Dob = dto.Dob,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    Phone = dto.Phone,
                    DeptId = dto.DeptId,
                    CurrentSemester = dto.CurrentSemester,
                    GroupId = 3,
                    StudentImg = imageName 
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                await SendStudentEmail(dto.Email, dto.StudentName, generatedPassword);
                return Ok(new
                {
                    success = true,
                    message = "Student registered successfully",
                    student = new
                    {
                        student.StudentId,
                        student.StudentName,
                        student.Email,
                        student.StudentImg,
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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

        [HttpPost("loginStudent")]
        public async Task<ActionResult> loginStudent([FromBody] LoginUserDTO loginUserDto)
        {
            var student = _context.Students.Include(u => u.Group).Where(u => u.Email == loginUserDto.Email).FirstOrDefault();
            if (student == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, student.Password))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Email and Password"
                });
            }

            var token = GenerateJwtTokenForStudent(student);

            return Ok(new
            {
                success = true,
                message = "Login Successfull.",
                token = token,
                roleId = student.GroupId,
                redirectUrl = GetRedirectUrl(student.GroupId)
            });
        }

        [HttpPost("loginFaculty")]
        public async Task<ActionResult> loginFaculty([FromBody] LoginUserDTO loginUserDto)
        {
            var faculty = _context.Faculties.Include(u => u.Group).Where(u => u.Email == loginUserDto.Email).FirstOrDefault();
            if (faculty == null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, faculty.Password))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid Email and Password"
                });
            }

            var token = GenerateJwtTokenForFaculty(faculty);

            return Ok(new
            {
                success = true,
                message = "Login Successfull.",
                token = token,
                roleId = faculty.GroupId,
                redirectUrl = GetRedirectUrl(faculty.GroupId)
            });
        }

        private string GenerateJwtTokenForStudent(Student student)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, student.Email),
            new Claim("StudentUserId", student.StudentId.ToString()),
            new Claim("Role", student.Group.Role.ToString()),
            new Claim("GroupId", student.GroupId.ToString()),
        };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        private string GenerateJwtTokenForFaculty(Faculty faculty)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, faculty.Email),
            new Claim("FacultyUserId", faculty.FacultyId.ToString()),
            new Claim("Role", faculty.Group.Role.ToString()),
            new Claim("GroupId", faculty.GroupId.ToString()),
        };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GetRedirectUrl(int roleId)
        {
            return roleId switch
            {
                1 => "/admin/admindashboard",
                2 => "/faculty/facultydashboard",
                3 => "/student/Studentdashboard",
                _ => "/"
            };
        }


        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
