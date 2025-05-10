using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class StudentFee
{
    public int FeeId { get; set; }

    public int StudentId { get; set; }

    public int FeeStructureId { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual FeeStructure FeeStructure { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
