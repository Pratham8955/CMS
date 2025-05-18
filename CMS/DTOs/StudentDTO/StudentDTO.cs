using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.StudentDTO
{
    public class StudentDTO
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

        // ✅ Now Required
        [Required(ErrorMessage = "10th marksheet is required.")]
        public IFormFile TenthMarksheet { get; set; } = null!;

        [Required(ErrorMessage = "12th marksheet is required.")]
        public IFormFile TwelfthMarksheet { get; set; } = null!;

        [Required]
        public string TenthSchool { get; set; } = null!;

        [Required]
        public int TenthPassingYear { get; set; }

        [Required]
        public decimal TenthPercentage { get; set; }

        [Required]
        public string TwelfthSchool { get; set; } = null!;

        [Required]
        public int TwelfthPassingYear { get; set; }

        [Required]
        public decimal TwelfthPercentage { get; set; }


    }
}
