﻿using CMS.DTOs.CourseContentDTO;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.FacultyController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseContentController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;


        public CourseContentController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("upload-course-content")]
        public async Task<IActionResult> UploadCourseContent([FromForm] CourseContentDTO dto)
        {
            try
            {
                var isAssigned = await _context.FacultySubjects
                    .AnyAsync(fs => fs.FacultyId == dto.FacultyId && fs.SubjectId == dto.SubjectId);

                if (!isAssigned)
                    return BadRequest(new { success = false, message = "Faculty not assigned to this subject." });

                if (dto.PdfFile == null || !dto.PdfFile.FileName.EndsWith(".pdf"))
                    return BadRequest(new { success = false, message = "Only PDF files are allowed." });

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "course-content");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.PdfFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PdfFile.CopyToAsync(stream);
                }

                var content = new CourseContent
                {
                    SubjectId = dto.SubjectId,
                    Title = dto.Title,
                    Description = dto.Description,
                    FilePath = Path.Combine("uploads", "course-content", fileName).Replace("\\", "/"),
                    UploadDate = DateTime.Now
                };

                _context.CourseContents.Add(content);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "PDF uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contents = await _context.CourseContents
                .Select(c => new
                {
                    c.ContentId,
                    c.Title,
                    c.Description,
                    c.FilePath,
                    c.SubjectId,
                    c.UploadDate
                }).ToListAsync();

            return Ok(contents);
        }

        [HttpGet("GetByIdforstudent/{id}")]
        public async Task<IActionResult> GetByIdforstudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }
            int studepid = student.DeptId;
            int stusemid = student.CurrentSemester;

            var content = await _context.CourseContents.Include(cc => cc.Subject).Where(cc => cc.Subject.DeptId == studepid && cc.Subject.SemId == stusemid).Select(cc => new
            {
                cc.SubjectId,
                cc.Title,
                cc.FilePath,
                cc.Subject.SubjectName

            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Content fetch successfully.",
                Content = content,
            });
        }

        [HttpGet("GetByIdforFaculty/{id}")]
        public async Task<IActionResult> GetByIdforFaculty(int id)
        {
            try
            {
                var facultyExists = await _context.Faculties.AnyAsync(f => f.FacultyId == id);
                if (!facultyExists)
                {
                    return NotFound("Faculty not found.");
                }

               
                var content = await (from cc in _context.CourseContents
                                     join fs in _context.FacultySubjects
                                         on cc.SubjectId equals fs.SubjectId
                                     join s in _context.Subjects
                                         on cc.SubjectId equals s.SubjectId
                                     where fs.FacultyId == id
                                     select new
                                     {
                                         cc.SubjectId,
                                         cc.Title,
                                         cc.FilePath,
                                         SubjectName = s.SubjectName,
                                         cc.UploadDate,
                                         cc.Description,
                                         cc.ContentId
                                     }).ToListAsync();

                if (content == null || !content.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No subjects assigned to this faculty.",
                        Content = new List<object>()
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Faculty content fetched successfully.",
                    Content = content
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred: " + ex.Message
                });
            }
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CourseContentDTO dto)
        {
            var content = await _context.CourseContents.FindAsync(id);
            if (content == null)
                return NotFound();

            var isAssigned = await _context.FacultySubjects
                .AnyAsync(fs => fs.FacultyId == dto.FacultyId && fs.SubjectId == dto.SubjectId);
            if (!isAssigned)
                return BadRequest(new { success = false, message = "Faculty not assigned to this subject." });

            // Delete old file if a new one is uploaded
            if (dto.PdfFile != null && dto.PdfFile.FileName.EndsWith(".pdf"))
            {
                // Get the old file path
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", content.FilePath ?? "");

                // Check if old file exists, if so, delete it
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { success = false, message = $"Failed to delete old file: {ex.Message}" });
                    }
                }
                else
                {
                    // If file doesn't exist, log a warning
                    Console.WriteLine($"Warning: Old file at {oldFilePath} not found.");
                }

                // Save the new file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "course-content");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.PdfFile.FileName);
                var newFilePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.PdfFile.CopyToAsync(stream);
                }

                content.FilePath = Path.Combine("uploads", "course-content", fileName).Replace("\\", "/");
            }

            // Update other fields
            content.Title = dto.Title;
            content.Description = dto.Description;
            content.SubjectId = dto.SubjectId;
            content.UploadDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Content updated successfully." });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var content = await _context.CourseContents.FindAsync(id);
            if (content == null)
                return NotFound();

            // Optionally delete file from disk
            var filePath = Path.Combine("wwwroot", content.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.CourseContents.Remove(content);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Content deleted." });
        }

    }
}
