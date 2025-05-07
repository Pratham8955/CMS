using CMS.DTOs.CourseContentDTO;
using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpPost("add-course-content")]
        //public async Task<IActionResult> AddCourseContent([FromForm] CourseContentDTO courseContentDto, IFormFile pdfFile)
        //{
        //    // Retrieve faculty ID from the request body (sent by frontend)
        //    var facultyId = courseContentDto.FacultyId;
        //    var subjectId = courseContentDto.SubjectId;

        //    // Check if the faculty is assigned to the subject
        //    var isAssignedToSubject = await _context.FacultySubjects
        //        .Where(s => s.SubjectId == subjectId && s.FacultyId == facultyId)
        //        .AnyAsync();

        //    if (!isAssignedToSubject)
        //    {
        //        return Forbid("You are not assigned to this subject.");
        //    }

        //    if (pdfFile == null || pdfFile.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    // Define the path to save the uploaded file (can be stored as URL or file system path)
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", pdfFile.FileName);

        //    // Ensure the directory exists
        //    var directory = Path.GetDirectoryName(filePath);
        //    if (!Directory.Exists(directory))
        //    {
        //        Directory.CreateDirectory(directory);
        //    }

        //    // Save the file to disk
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await pdfFile.CopyToAsync(stream);
        //    }

        //    // Create and save the course content entity to the database
        //    var courseContent = new CourseContent
        //    {
        //        Title = courseContentDto.Title,
        //        Description = courseContentDto.Description,
        //        FilePath = filePath,  // Save the URL or path of the uploaded PDF
        //        UploadDate = DateTime.Now,
        //        SubjectId = subjectId  // Linking the content to the subject
        //    };

        //    _context.CourseContents.Add(courseContent);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Course content added successfully.", CourseContent = courseContent });
        //}

    }
}
