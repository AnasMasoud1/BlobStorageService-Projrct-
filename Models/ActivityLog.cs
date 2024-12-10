using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class ActivityLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User? User { get; set; }
}
