using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class Permission
{
    public int Id { get; set; }

    public int? FileId { get; set; }

    public int? UserId { get; set; }

    public string? PermissionType { get; set; }

    public virtual File? File { get; set; }

    public virtual User? User { get; set; }
}
