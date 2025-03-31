using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs
{
    public class FacultySubjectsDTO
    {
        public int? FacultyId { get; set; }

        public int? SubjectId { get; set; }

        [Required]
        public int SemId { get; set; }

    }
}
