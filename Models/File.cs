using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class File
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long Size { get; set; }

    public DateTime CreatedDate { get; set; }

    public int UploadedBy { get; set; }

    public virtual ICollection<FileMetadatum> FileMetadata { get; set; } = new List<FileMetadatum>();

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual User UploadedByNavigation { get; set; } = null!;
}
