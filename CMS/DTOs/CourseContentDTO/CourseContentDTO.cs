namespace CMS.DTOs.CourseContentDTO
{
    public class CourseContentDTO
    {
        public int ContentId { get; set; }
        public int? SubjectId { get; set; }  

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public IFormFile PdfFile { get; set; } = null!;  

        public int FacultyId { get; set; }
    }
}
