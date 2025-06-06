﻿using System;
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
            var faculty = await _context.Faculties.Where(fe=>fe.FacultyName!="Admin").Select(f => new
            {
                f.FacultyId,
                f.FacultyName,
                f.Email,
                f.Doj,
                f.Gender,
                f.Qualification,
                f.Experience,
                f.Password,
                f.DeptId,
                f.GroupId,
                f.FacultyImg,
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
        public async Task<IActionResult> AddFaculty([FromForm] FacultyDTO dto)
        {
            try
            {
                if (_context.Students.Any(u => u.Email == dto.Email) ||
                    _context.Faculties.Any(f => f.Email == dto.Email))
                {
                    throw new Exception("Email already exists.");
                }
                string? imageName = null;

                // Save image if provided
                if (dto.FacultyImg != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Faculty");
                    Directory.CreateDirectory(uploadsFolder); // ensure folder exists

                    imageName = Guid.NewGuid().ToString() + Path.GetExtension(dto.FacultyImg.FileName);
                    string filePath = Path.Combine(uploadsFolder, imageName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.FacultyImg.CopyToAsync(fileStream);
                    }
                }


                
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
                    GroupId = 2, // Fixed for faculty,
                    FacultyImg = imageName,
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
        public async Task<IActionResult> UpdateFaculty(int id, [FromForm] UpdateFacultyDTO updatefaculty)
        {
            var faculty = await _context.Faculties.FindAsync(id);
            if (faculty == null)
            {
                return NotFound($"Faculty with Id {id} Not Found");
            }

            // Check if a new image is provided
            if (updatefaculty.FacultyImg != null)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(faculty.FacultyImg))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Faculty", faculty.FacultyImg);
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
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Faculty");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string newImageName = Guid.NewGuid().ToString() + Path.GetExtension(updatefaculty.FacultyImg.FileName);
                string newFilePath = Path.Combine(uploadsFolder, newImageName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await updatefaculty.FacultyImg.CopyToAsync(fileStream);
                }

                faculty.FacultyImg = newImageName; // Update the FacultyImg field
            }

            // Update the rest of the faculty fields
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
                return Ok(new
                {
                    success = true,
                    message = "Faculty updated successfully",
                    faculty = new
                    {
                        faculty.FacultyId,
                        faculty.FacultyName,
                        faculty.Email,
                        faculty.Doj,
                        faculty.Gender,
                        faculty.Qualification,
                        faculty.Experience,
                        faculty.DeptId,
                        FacultyImgUrl = Url.Content($"~/uploads/Faculty/{faculty.FacultyImg}")
                    }
                }); 
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
                // Delete faculty image file if exists
                if (!string.IsNullOrEmpty(faculty.FacultyImg))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Faculty", faculty.FacultyImg);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

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
            var faculty = await _context.Faculties.Where(f => f.FacultyId == id).Select(f => new
            {
                f.FacultyId,
                f.FacultyName,
                f.Email,
                f.Doj,
                f.Gender,
                f.Qualification,
                f.Experience,
                f.DeptId,
                f.FacultyImg,
                Depname = f.Dept.DeptName,
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


        [HttpGet("GetFacultyBystudentId/{id}")]
        public async Task<IActionResult> GetFacultyBystudentId(int id)
        {
            var stud = await _context.Students.FindAsync(id);
            if (stud == null)
            {
                return NotFound($"Student with Id {id} not found");
            }

            var studdep = stud.DeptId;

            var faculty = await _context.Faculties
                .Where(f => f.DeptId == studdep)
                .Select(f => new
                {
                    f.FacultyId,
                    f.FacultyName,
                    f.Email,
                    f.Gender
                }).ToListAsync();

            if (!faculty.Any())
            {
                return NotFound($"No faculty found for department Id {studdep}");
            }

            return Ok(new
            {
                success = true,
                message = "Faculty fetched successfully.",
                faculty = faculty,
            });
        }




    }
}
