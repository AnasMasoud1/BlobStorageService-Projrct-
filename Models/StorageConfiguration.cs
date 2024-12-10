using System;
using System.Collections.Generic;

namespace BlobStorageService.Models;

public partial class StorageConfiguration
{
    public int Id { get; set; }

    public string StorageType { get; set; } = null!;

    public string ConfigurationDetails { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
