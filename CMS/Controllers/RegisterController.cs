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

        // GET: api/Students
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        //{
        //    return await _context.Students.ToListAsync();
        //}

        // GET: api/Students/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Student>> GetStudent(int id)
        //{
        //    var student = await _context.Students.FindAsync(id);

        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    return student;
        //}

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutStudent(int id, Student student)
        //{
        //    if (id != student.StudentId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(student).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!StudentExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Student>> PostStudent(Student student)
        //{
        //    _context.Students.Add(student);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        //}

        // DELETE: api/Students/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteStudent(int id)
        //{
        //    var student = await _context.Students.FindAsync(id);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Students.Remove(student);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
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


        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromForm] StudentDTO dto)
        {
            try
            {
                if (_context.Students.Any(u => u.Email == dto.Email))
                {
                    throw new Exception("Email Already Exists.");
                }
                // ✅ Step 1: Check if image is uploaded
                //if (studentImage == null || studentImage.Length == 0)
                //{
                //    return BadRequest(new { success = false, message = "Please upload a student image." });
                //}

                //// ✅ Step 2: Validate allowed file formats
                //var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                //var extension = Path.GetExtension(studentImage.FileName).ToLower();

                //if (!allowedExtensions.Contains(extension))
                //{
                //    return BadRequest(new { success = false, message = "Invalid image format. Only .jpg, .jpeg, .png allowed." });
                //}

                //// ✅ Step 3: Limit file size to 2MB
                //const int maxFileSize = 2 * 1024 * 1024;
                //if (studentImage.Length > maxFileSize)
                //{
                //    return BadRequest(new { success = false, message = "Image size must be less than 2MB." });
                //}

                //// ✅ Step 4: Generate a unique filename using timestamp
                //string uniqueFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";

                //// ✅ Step 5: Set path to save image
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/student-images");
                //if (!Directory.Exists(uploadsFolder))
                //{
                //    Directory.CreateDirectory(uploadsFolder);
                //}

                //string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //// ✅ Step 6: Save the file to the server
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await studentImage.CopyToAsync(stream);
                //}

                var student = new Student
                {
                    StudentName = dto.StudentName,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Dob = dto.Dob,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    City = dto.City,
                    State = dto.State,
                    Phone = dto.Phone,
                    DeptId = dto.DeptId,
                    CurrentSemester = dto.CurrentSemester,
                    GroupId = dto.GroupId,
                    //StudentImg = uniqueFileName,
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Successfully inserted",
                    student = new
                    {
                        student.StudentId,
                        student.GroupId,
                        student.StudentName,
                        student.Dob,
                        student.Gender,
                        student.Email,
                        student.Address,
                        student.City,
                        student.State,
                        student.Phone,
                    }
                });
            }
            catch (FormatException)
            {
                return BadRequest(new { success = false, message = "Invalid date format. Please use yyyy-MM-dd." });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message,
                    details = ex.InnerException?.Message // Capture the real issue
                });
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
