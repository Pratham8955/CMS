using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.DTOs.StudentFeesType
{
    public class StudentFeesTypeDTO
    {
        public int FeetypeId { get; set; }

        [Required]
        public int FeeStructureId { get; set; } 

        public int? FeeId { get; set; } 

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tuition Fees must be a positive number.")]
        public decimal TuitionFees { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Lab Fees must be a positive number.")]
        public decimal LabFees { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "College Ground Fee must be a positive number.")]
        public decimal CollegeGroundFee { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Internal Exam Fee must be a positive number.")]
        public decimal InternalExam { get; set; }

        public DateTime? TransactionDate { get; set; }

        public int? StudentId { get; set; }
    }
}
