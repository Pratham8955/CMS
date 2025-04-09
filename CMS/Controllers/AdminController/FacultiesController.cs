using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMS.Models;
using CMS.DTOs;
using System.Text.RegularExpressions;
using CMS.DTOs.FacultyDTO;
using System.Net.Mail;
using System.Net;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;


        public FacultiesController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //-------------------------
        //Faculties API

        [HttpGet("GetFaculties")]

        public async Task<IActionResult> GetFaculties()
        {
            var faculty = await _context.Faculties.Select(f => new FacultyDTO
            {
                FacultyId = f.FacultyId,
                FacultyName = f.FacultyName,
                Email = f.Email,
                Doj = f.Doj,
                Gender = f.Gender,
                Qualification = f.Qualification,
                Experience = f.Experience,
                Password = f.Password,
                DeptId = f.DeptId,
                GroupId = f.GroupId,
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Faculty fetch successfully.",
                Faculty = faculty,
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


        private async Task<bool> SendFacultyEmail(string toEmail, string facultyName, string plainPassword)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailTemplate.html");
                string emailBody = await System.IO.File.ReadAllTextAsync(templatePath);

                emailBody = emailBody
                    .Replace("{{FacultyName}}", facultyName)
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
                    Subject = "Your Faculty Login Credentials",
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



        [HttpPost("AddFaculty")]
        public async Task<IActionResult> AddFaculty([FromBody] FacultyDTO dto)
        {
            try
            {
                if (_context.Students.Any(u => u.Email == dto.Email) ||
                    _context.Faculties.Any(f => f.Email == dto.Email))
                {
                    throw new Exception("Email already exists.");
                }

                // Auto-generate and hash password
                string generatedPassword = GenerateSecurePassword();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

                var faculty = new Faculty
                {
                    FacultyName = dto.FacultyName,
                    Email = dto.Email,
                    Doj = dto.Doj,
                    Gender = dto.Gender,
                    Qualification = dto.Qualification,
                    Experience = dto.Experience,
                    Password = hashedPassword,
                    DeptId = dto.DeptId,
                    GroupId = 2 // Fixed for faculty
                };

                _context.Faculties.Add(faculty);
                await _context.SaveChangesAsync();

                // Send credentials via email
                await SendFacultyEmail(dto.Email, dto.FacultyName, generatedPassword);

                return Ok(new
                {
                    success = true,
                    message = "Faculty added successfully. Credentials sent to email.",
                    faculty = new
                    {
                        faculty.FacultyId,
                        faculty.FacultyName,
                        faculty.Email,
                        faculty.Doj,
                        faculty.Gender,
                        faculty.Qualification,
                        faculty.Experience,
                        faculty.DeptId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        [HttpPost("UpdateFaculty/{id}")]
        public async Task<IActionResult> UpdateFaculty(int id, [FromBody] UpdateFacultyDTO updatefaculty)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound($"Faculty with Id {id} Not Found");
            }


            faculty.FacultyName = updatefaculty.FacultyName;
            faculty.Email = updatefaculty.Email;
            faculty.Doj = updatefaculty.Doj;
            faculty.Gender = updatefaculty.Gender;
            faculty.Qualification = updatefaculty.Qualification;
            faculty.Experience = updatefaculty.Experience;
            faculty.DeptId = updatefaculty.DeptId;
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

        [HttpDelete("DeleteFaculty")]

        public async Task<IActionResult> DeleteFaculty(int id)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound($"Faculty with Id {id} Not Found");
            }
            try
            {
                _context.Faculties.Remove(faculty);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Faculty: {ex.Message}");
            }
        }



        [HttpGet("GetFacultyById/{id}")]

        public async Task<IActionResult> GetFacultiesById(int id)
        {
            var faculty = await _context.Faculties.Where(f => f.FacultyId == id).Select(f => new FacultyDTO
            {
                FacultyId = f.FacultyId,
                FacultyName = f.FacultyName,
                Email = f.Email,
                Doj = f.Doj,
                Gender = f.Gender,
                Qualification = f.Qualification,
                Experience = f.Experience,
                DeptId = f.DeptId,
            }).ToListAsync();
            if (faculty == null)
            {
                return NotFound($"Faculty with Id {id} not found");
            }

            return Ok(new
            {
                success = true,
                message = "Faculty fetch successfully.",
                Faculty = faculty,
            });
        }



    }
}
