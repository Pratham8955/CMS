using System.ComponentModel.DataAnnotations;
using CMS.DTOs.StudentFeesType;

namespace CMS.DTOs.StudentFeesDTO
{
    public class StudentFessDTO
    {
        public int FeeId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int FeeStructureId { get; set; }

        public decimal PaidAmount { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        
        public string? Status { get; set; }

        public string? TransactionId { get; set; }
        
        public DateTime? PaymentDate { get; set; }

    }
}
