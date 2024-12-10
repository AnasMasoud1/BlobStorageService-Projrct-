using BlobStorageService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // تحديد مسار ملف config لتهيئة الاتصال
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // مسار المجلد الحالي
            .AddJsonFile("appsettings.json")  // تحميل الإعدادات من ملف appsettings.json
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection"); // جلب السلسلة
        optionsBuilder.UseSqlServer(connectionString); // ربط DbContext مع SQL Server

        return new ApplicationDbContext(optionsBuilder.Options); // إعادة الكائن DbContext
    }
}
