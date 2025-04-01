using System.Data;
using CMS.DTOs;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //-------------------------------------
        //DepartmentsApi
        [HttpGet("GetDepartment")]
        public async Task<IActionResult> GetDepartment()
        {
            var department = await _context.Departments.Select(s => new DepartmentsDTO
            {
                DeptId = s.DeptId,
                DeptName = s.DeptName,
                HeadOfDept = s.HeadOfDept,
            }).ToListAsync();
            return Ok(new
            {
                success = true,
                message = "Departments fetch successfully.",
                Department = department,
            });
        }

        [HttpPost("AddDepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentsDTO addDepartment)
        {
            try
            {
                var department = new Department
                {
                    DeptName = addDepartment.DeptName,
                    HeadOfDept = addDepartment.HeadOfDept,
                };
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Department Added Successfully",
                    Department = new
                    {
                        department.DeptName,
                        department.HeadOfDept,
                    },
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPost("UpdateDepartment/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentsDTO updateDepartment)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound($"Departments with Id {id} Not Found");
            }
            department.DeptName = updateDepartment.DeptName;
            department.HeadOfDept = updateDepartment.HeadOfDept;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Department: {ex.Message}");
            }
        }

        [HttpDelete("DeleteDepartment")]

        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound($"Departments with Id {id} Not Found");
            }
            try
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Department: {ex.Message}");
            }
        }



        //-------------------------
        //Faculties API

        [HttpGet("GetFaculties")]

        public async Task<IActionResult> GetFaculties()
        {
            var faculty = await _context.Faculties.Select(f => new FacultyDTO
            {
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



        [HttpPost("AddFaculty")]
        public async Task<IActionResult> AddFaculty([FromBody] FacultyDTO dto)
        {
            try
            {
                if (_context.Students.Any(u => u.Email == dto.Email))
                {
                    throw new Exception("Email Already Exists.");
                }

                var faculty = new Faculty
                {

                    FacultyName = dto.FacultyName,
                    Email = dto.Email,
                    Doj = dto.Doj,
                    Gender = dto.Gender,
                    Qualification = dto.Qualification,
                    Experience = dto.Experience,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    DeptId = dto.DeptId,
                    GroupId = 2,

                };
                _context.Faculties.Add(faculty);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Successfully inserted",
                    faculty = new
                    {
                        faculty.FacultyId,
                        faculty.GroupId,
                        faculty.DeptId,
                        faculty.FacultyName,
                        faculty.Email,
                        faculty.Doj,
                        faculty.Gender,
                        faculty.Qualification,
                        faculty.Experience,
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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



        //-------------------------
        //Subjects API

        [HttpGet("GetSubjects")]

        public async Task<IActionResult> GetSubjects()
        {
            var subject = await _context.Subjects.Select(s => new SubjectDTO
            {
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
            try
            {
                if (_context.Subjects.Any(u => u.SubjectName == dto.SubjectName))
                {
                    throw new Exception("Subjects Already Exists.");
                }

                var subject = new Subject
                {

                    SubjectName = dto.SubjectName,
                    DeptId = dto.DeptId,
                    SemId = dto.SemId,
                };
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Successfully inserted",
                    Subject = new
                    {
                        subject.SubjectName,
                        subject.DeptId,
                        subject.SemId
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPost("UpdateSubject/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectDTO updateSubject)
        {
            var subject = await _context.Subjects.FindAsync(id);
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

        [HttpDelete("DeleteSubject")]

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



        [HttpPost("AddFacultySubjects")]
        public async Task<IActionResult> AddFacultySubjects([FromBody] FacultySubjectsDTO dto)
        {
            try
            {
                // Check if faculty is already assigned to the subject
                var existingAssignment = await _context.FacultySubjects
                    .FirstOrDefaultAsync(fs => fs.FacultyId == dto.FacultyId && fs.SubjectId == dto.SubjectId && fs.SemId == dto.SemId);

                if (existingAssignment != null)
                {
                    return Conflict(new { success = false, message = "Faculty is already assigned to this subject." });
                }

                var facsubject = new FacultySubject
                {
                    FacultyId = dto.FacultyId,
                    SubjectId = dto.SubjectId,
                    SemId = dto.SemId
                };

                _context.FacultySubjects.Add(facsubject);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Successfully inserted",
                    FacultySubject = new
                    {
                        facsubject.FacultyId,
                        facsubject.SubjectId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred: " + ex.Message });
            }
        }



        [HttpPost("UpdateFacultySubject/{id}")]
        public async Task<IActionResult> UpdateFacultySubject(int id, [FromBody] FacultySubjectsDTO updateFacultySubject)
        {
            var facsubject = await _context.FacultySubjects.FindAsync(id);
            if (facsubject == null)
            {
                return NotFound($"Faculty Subject with Id {id} Not Found");
            }
            facsubject.FacultyId = updateFacultySubject.FacultyId;
            facsubject.SubjectId = updateFacultySubject.SubjectId;
            facsubject.SemId = updateFacultySubject.SemId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating Faculty Subject: {ex.Message}");
            }
        }

        [HttpDelete("DeleteFacultySubject")]

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



        //Semester


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
                Semsester = semester,
            });
        }
    }
}
