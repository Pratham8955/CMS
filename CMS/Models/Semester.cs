using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Semester
{
    public int SemId { get; set; }

    public string SemName { get; set; } = null!;

    public virtual ICollection<FacultySubject> FacultySubjects { get; set; } = new List<FacultySubject>();

    public virtual ICollection<FeeStructure> FeeStructures { get; set; } = new List<FeeStructure>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
