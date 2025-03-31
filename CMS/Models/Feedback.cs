using System;
using System.Collections.Generic;

namespace CMS.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? Timestamp { get; set; }
}
