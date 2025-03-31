using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs
{
    public class DepartmentsDTO
    {
        [Required]
        public string DeptName { get; set; } = null!;

        public int? HeadOfDept { get; set; }
    }
}
