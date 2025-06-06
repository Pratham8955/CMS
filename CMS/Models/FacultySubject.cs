﻿using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class FacultySubject
{
    public int FacultySubjectId { get; set; }

    public int? FacultyId { get; set; }

    public int? SubjectId { get; set; }

    public int SemId { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual Semester Sem { get; set; } = null!;

    public virtual Subject? Subject { get; set; }
}
