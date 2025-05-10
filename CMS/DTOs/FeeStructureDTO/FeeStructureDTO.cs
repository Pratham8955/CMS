using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.FeeStructureDTO
{
    public class FeeStructureDTO
    {
        [Required]
        public int DeptId { get; set; }

        [Required]
        public int SemId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Default amount must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal DefaultAmount { get; set; }

        public string? FeeStructureDescription { get; set; }
        public int FeeStructureId { get; set; }
    }
}
