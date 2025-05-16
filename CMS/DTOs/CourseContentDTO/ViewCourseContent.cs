namespace CMS.DTOs.CourseContentDTO
{
    public class ViewCourseContent
    {
        public int ContentId { get; set; }

        public int? SubjectId { get; set; }

        public string Title { get; set; } = null!;

        public string FilePath { get; set; } = null!;
        public DateTime? UploadDate { get; set; }
        public int FacultyId { get; set; }
        public string? Description { get; set; }

    }
}
