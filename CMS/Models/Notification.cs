using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime SentDate { get; set; }

    public bool IsRead { get; set; }

    public virtual Student? Student { get; set; }
}
