using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class FeeStructure
{
    public int FeeStructureId { get; set; }

    public int DeptId { get; set; }

    public int SemId { get; set; }

    public decimal DefaultAmount { get; set; }

    public string? FeeStructureDescription { get; set; }

    public virtual Department Dept { get; set; } = null!;

    public virtual Semester Sem { get; set; } = null!;

    public virtual ICollection<StudentFee> StudentFees { get; set; } = new List<StudentFee>();

    public virtual ICollection<StudentFeesType> StudentFeesTypes { get; set; } = new List<StudentFeesType>();
}
