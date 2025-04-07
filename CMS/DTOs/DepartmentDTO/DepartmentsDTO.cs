using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.DepartmentDTO
{
    public class DepartmentsDTO
    {

        public int DeptId { get; set; }

        [Required]
        public string DeptName { get; set; } = null!;

        public int? HeadOfDept { get; set; }
    }
}
