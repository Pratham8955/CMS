using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs
{
    public class FacultyDTO
    {

        [Required]
        public string FacultyName { get; set; } = null!;

        [Required,EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public DateOnly Doj { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        public string? Qualification { get; set; }

        [Required]
        public string? Experience { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        public int DeptId { get; set; }

        [Required]
        public int GroupId { get; set; }

    }
}
