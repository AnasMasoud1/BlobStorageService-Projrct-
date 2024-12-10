using Microsoft.EntityFrameworkCore;

namespace BlobStorageService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // تعريف جدول المستخدمين
        public DbSet<User> Users { get; set; }
    }

    // نموذج المستخدم
    //public class User
    //{
    //    public int Id { get; set; }
    //    public string Username { get; set; }
    //    public string Password { get; set; }  // ستتم معالجتها باستخدام التجزئة في المستقبل
    //    public string Role { get; set; }  // إضافة دور المستخدم
    //}
}
