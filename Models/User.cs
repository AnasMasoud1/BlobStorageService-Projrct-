using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class User
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
