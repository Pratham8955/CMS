using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class GroupMaster
{
    public int GroupId { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
