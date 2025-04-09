using CMS.DTOs.SubjectDTO;
using CMS.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;
        public SubjectController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //-------------------------
        //Subjects API

        [HttpGet("GetSubjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var subject = await _context.Subjects.Select(s => new SubjectDTO
            {
                SubjectId = s.SubjectId,
                SubjectName = s.SubjectName,
                DeptId = s.DeptId,
                SemId = s.SemId
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Subjects fetch successfully.",
                Subject = subject,
            });
        }



        [HttpPost("AddSubjects")]
        public async Task<IActionResult> AddSubjects([FromBody] SubjectDTO dto)
        {
            if (_context.Subjects.Any(u => u.SubjectName == dto.SubjectName && u.DeptId == dto.DeptId && u.SemId == dto.SemId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Subject already exists for this department and semester."
                });
            }

            var subject = new Subject
            {
                SubjectName = dto.SubjectName,
                DeptId = dto.DeptId,
                SemId = dto.SemId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Subject inserted successfully.",
                Subject = new
                {
                    subject.SubjectName,
                    subject.DeptId,
                    subject.SemId
                }
            });
        }



        [HttpPost("UpdateSubject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectDTO updateSubject)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (_context.Subjects.Any(u => u.SubjectName == updateSubject.SubjectName && u.DeptId == updateSubject.DeptId && u.SemId == updateSubject.SemId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Subject already exists for this department and semester."
                });
            }
            if (subject == null)
            {
                return NotFound($"Subject with Id {id} Not Found");
            }
            subject.SubjectName = updateSubject.SubjectName;
            subject.DeptId = updateSubject.DeptId;
            subject.SemId = updateSubject.SemId;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Subject: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSubject/{id}")]

        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound($"Subject with Id {id} Not Found");
            }
            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Subjects {ex.Message}");
            }
        }
    }

}
