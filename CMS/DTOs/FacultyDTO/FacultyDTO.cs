using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.FacultyDTO
{
    public class FacultyDTO
    {
        public int FacultyId { get; set; }

        [Required]
        public string FacultyName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public DateOnly Doj { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        public string? Qualification { get; set; }

        [Required]
        public int? Experience { get; set; }

        // Optional — remove Required and MinLength
        public string? Password { get; set; }

        [Required]
        public int DeptId { get; set; }

        // Optional — hardcoded as 2 in backend
        public int GroupId { get; set; }

        [Required]
        public IFormFile FacultyImg { get; set; }=null!;
    }
}
