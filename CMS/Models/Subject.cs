using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int DeptId { get; set; }

    public int SemId { get; set; }

    public virtual ICollection<CourseContent> CourseContents { get; set; } = new List<CourseContent>();

    public virtual Department Dept { get; set; } = null!;

    public virtual ICollection<FacultySubject> FacultySubjects { get; set; } = new List<FacultySubject>();

    public virtual Semester Sem { get; set; } = null!;
}
