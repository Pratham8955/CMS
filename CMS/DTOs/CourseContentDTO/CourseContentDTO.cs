namespace CMS.DTOs.CourseContentDTO
{
    public class CourseContentDTO
    {
        public int ContentId { get; set; }

        public int? SubjectId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string FilePath { get; set; } = null!;

        public DateTime? UploadDate { get; set; }
    }
}
