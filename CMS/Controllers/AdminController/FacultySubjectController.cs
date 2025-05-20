using CMS.DTOs.FacultySubjectDTO;
using CMS.DTOs.SemesterDTO;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultySubjectController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public FacultySubjectController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //-------------------------
        //FacultySubjects API

        [HttpGet("GetFacultySubjects")]

        public async Task<IActionResult> GetFacultySubjects()
        {
            var Facsubject = await _context.FacultySubjects.Select(s => new FacultySubjectsDTO
            {
                FacultyId = s.FacultyId,
                SubjectId = s.SubjectId,
                SemId = s.SemId,
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "FacultySubjects fetch successfully.",
                FacultySubject = Facsubject,
            });
        }
        [HttpGet("GetFacultySubjectsForAssignedFaculty/{facultyid}")]

        public async Task<IActionResult> GetFacultySubjectsForAssignedFaculty(int facultyid)
        {
            var Facsubject = await _context.FacultySubjects.Where(f => f.FacultyId == facultyid).Select(s => new
            {
                s.FacultyId,
                s.SubjectId,
                s.SemId,
                subject = s.Subject.SubjectName,
                Depname = s.Subject.Dept.DeptName
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "FacultySubjects fetch successfully.",
                FacultySubject = Facsubject,
            });
        }


        //GetSubject according to sem and dep 
        [HttpGet("GetSubjectsByDepartmentAndSemester/{deptId}/{semId}")]
        public async Task<IActionResult> GetSubjectsByDepartmentAndSemester(int deptId, int semId)

        {
            var subjects = await _context.Subjects
                .Where(s => s.DeptId == deptId && s.SemId == semId)
                .Select(s => new
                {
                    s.SubjectId,
                    s.SubjectName
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                message = "Subjects fetched successfully.",
                Subjects = subjects
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



        //AssignSubject to faculty
        [HttpPost("AssignFacultySubject")]
        public async Task<IActionResult> AssignFacultySubject([FromBody] FacultySubjectsDTO model)
        {
            // Check if faculty is already assigned to the subject
            var existingAssignment = await _context.FacultySubjects
            .FirstOrDefaultAsync(fs => fs.FacultyId == model.FacultyId && fs.SubjectId == model.SubjectId && fs.SemId == model.SemId);

            if (existingAssignment != null)
            {
                return Conflict(new { success = false, message = "Faculty is already assigned to this subject." });
            }

            var subjectAlreadyAssigned = await _context.FacultySubjects
       .AnyAsync(fs => fs.SubjectId == model.SubjectId && fs.SemId == model.SemId);

            if (subjectAlreadyAssigned)
            {
                return Conflict(new { success = false, message = "This subject is already assigned to another faculty for this semester." });
            }

            if (model.FacultyId == null || model.SubjectId == null || model.SemId == 0)
                return BadRequest(new { success = false, message = "Invalid input" });

            var facultySubject = new FacultySubject
            {
                FacultyId = model.FacultyId.Value,
                SubjectId = model.SubjectId.Value,
                SemId = model.SemId
            };

            _context.FacultySubjects.Add(facultySubject);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Faculty assigned to subject successfully" });
        }


        [HttpPost("UpdateFacultySubject/{id}")]
        public async Task<IActionResult> UpdateFacultySubject(int id, [FromBody] FacultySubjectsDTO updateFacultySubject)
        {
            // Validate input
            if (updateFacultySubject.FacultyId == null || updateFacultySubject.SubjectId == null || updateFacultySubject.SemId == 0)
            {
                return BadRequest(new { success = false, message = "FacultyId, SubjectId, and SemId are required." });
            }

            // Find existing assignment
            var facsubject = await _context.FacultySubjects.FindAsync(id);
            if (facsubject == null)
            {
                return NotFound(new { success = false, message = $"Faculty Subject with Id {id} not found." });
            }

            // Optional: Check if the new faculty or subject exists
            var facultyExists = await _context.Faculties.AnyAsync(f => f.FacultyId == updateFacultySubject.FacultyId);
            var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == updateFacultySubject.SubjectId);

            if (!facultyExists)
            {
                return NotFound(new { success = false, message = $"Faculty with Id {updateFacultySubject.FacultyId} not found." });
            }

            if (!subjectExists)
            {
                return NotFound(new { success = false, message = $"Subject with Id {updateFacultySubject.SubjectId} not found." });
            }

            // Update the record
            facsubject.FacultyId = updateFacultySubject.FacultyId.Value;
            facsubject.SubjectId = updateFacultySubject.SubjectId.Value;
            facsubject.SemId = updateFacultySubject.SemId;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Faculty Subject assignment updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error updating Faculty Subject: {ex.Message}" });
            }
        }


        [HttpDelete("DeleteFacultySubject/{id}")]

        public async Task<IActionResult> DeleteFacultySubject(int id)
        {
            var facsubject = await _context.FacultySubjects.FindAsync(id);
            if (facsubject == null)
            {
                return NotFound($"Faculty Subject with Id {id} Not Found");
            }
            try
            {
                _context.FacultySubjects.Remove(facsubject);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Faculty Subjects {ex.Message}");
            }
        }

    }
}
