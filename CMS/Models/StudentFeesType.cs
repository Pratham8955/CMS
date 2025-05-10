using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class StudentFeesType
{
    public int FeetypeId { get; set; }

    public int FeeStructureId { get; set; }

    public decimal TuitionFees { get; set; }

    public decimal LabFees { get; set; }

    public decimal CollegeGroundFee { get; set; }

    public decimal InternalExam { get; set; }

    public virtual FeeStructure FeeStructure { get; set; } = null!;
}
