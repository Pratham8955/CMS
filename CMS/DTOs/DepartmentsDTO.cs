using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs
{
    public class DepartmentsDTO
    {

        public int DeptId { get; set; }

        [Required]
        public string DeptName { get; set; } = null!;

        public int? HeadOfDept { get; set; }
    }
}
