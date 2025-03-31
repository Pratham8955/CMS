using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly Doj { get; set; }

    public string Gender { get; set; } = null!;

    public string? Qualification { get; set; }

    public int? Experience { get; set; }

    public string Password { get; set; } = null!;

    public int DeptId { get; set; }

    public int GroupId { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual Department Dept { get; set; } = null!;

    public virtual ICollection<FacultySubject> FacultySubjects { get; set; } = new List<FacultySubject>();

    public virtual GroupMaster Group { get; set; } = null!;
}
