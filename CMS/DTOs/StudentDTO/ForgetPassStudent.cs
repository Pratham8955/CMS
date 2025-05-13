using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.StudentDTO
{
    public class ForgetPassStudent
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
