using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.StudentDTO
{
    public class UpdateStudentDTO
    {
        [Required]
        public string StudentName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public DateOnly Dob { get; set; }

        [Required]
        public string Gender { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string State { get; set; } = null!;

        [Required, Phone]
        public string Phone { get; set; } = null!;

        [Required]
        public int DeptId { get; set; }

        [Required]
        public int CurrentSemester { get; set; }


        [Required]
        public IFormFile StudentImg { get; set; } = null!;

    }
}
