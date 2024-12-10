using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class FileMetadatum
{
    public int Id { get; set; }

    public int FileId { get; set; }

    public string ContentType { get; set; } = null!;

    public bool IsEncrypted { get; set; }

    public DateTime LastAccessedDate { get; set; }

    public virtual File File { get; set; } = null!;
}
