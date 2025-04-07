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
