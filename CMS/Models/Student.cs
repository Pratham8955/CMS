﻿using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int DeptId { get; set; }

    public int CurrentSemester { get; set; }

    public int GroupId { get; set; }

    public string? StudentImg { get; set; }

    public string? TenthSchool { get; set; }

    public int? TenthPassingYear { get; set; }

    public decimal? TenthPercentage { get; set; }

    public string? Tenthmarksheet { get; set; }

    public string? TwelfthSchool { get; set; }

    public int? TwelfthPassingYear { get; set; }

    public decimal? TwelfthPercentage { get; set; }

    public string? TwelfthMarksheet { get; set; }

    public virtual Semester CurrentSemesterNavigation { get; set; } = null!;

    public virtual Department Dept { get; set; } = null!;

    public virtual GroupMaster Group { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<StudentFee> StudentFees { get; set; } = new List<StudentFee>();
}
