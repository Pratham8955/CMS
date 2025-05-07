using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.FacultyDTO
{
    public class UpdateFacultyDTO
    {
        [Required]
        public string FacultyName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public DateOnly Doj { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        public string? Qualification { get; set; }

        public int? Experience { get; set; } 

        [Required]
        public int DeptId { get; set; }


        [Required]
        public IFormFile FacultyImg { get; set; } = null!;
    }
}
