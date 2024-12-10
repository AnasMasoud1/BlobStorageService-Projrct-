//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();


//// إعداد المصادقة JWT
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Anas1234"))
//        };
//    });

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthorization();

//app.MapControllers();

//app.Run();


//using BlobStorageService.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);  // يجب أن يكون هذا السطر أولاً

//// إضافة الخدمات إلى الحاوية
//builder.Services.AddControllers();

//// إعداد المصادقة باستخدام JWT Bearer
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Anas1234"))  // استخدم مفتاح سرّي آمن
//        };
//    });

//// إضافة خدمة التخزين المحلي
//builder.Services.AddSingleton<IStorageBackend>(provider =>
//    new LocalFileSystemStorage("C:\\BlobStorage"));  // ضع هنا المسار الذي ترغب بتخزين الملفات فيه

//// إضافة Swagger للتوثيق التفاعلي للـ API
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter JWT with Bearer into field",
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            new string[] { }
//        }
//    });
//});

//var app = builder.Build();

//// تكوين سير عمل طلبات HTTP
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();  // إضافة Swagger في بيئة التطوير
//    app.UseSwaggerUI(); // واجهة المستخدم لـ Swagger
//}

//app.UseRouting();  // توجيه الطلبات إلى المسارات المناسبة

//app.UseAuthentication();  // تطبيق المصادقة
//app.UseAuthorization();   // تطبيق التفويض

//app.MapControllers();  // ربط الـ Controllers بالمسارات

//app.Run();  // تشغيل التطبيق


using BlobStorageService.Models;
using BlobStorageService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات قاعدة البيانات (DbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// إضافة الخدمات إلى الحاوية
builder.Services.AddControllers();

// إعداد المصادقة باستخدام JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "BlobStorageAPI",
            ValidAudience = "BlobStorageApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Anas1234!@#StrongSecretKey123456789012"))
        };
    });

// تسجيل MinioStorageService كـ IStorageBackend
builder.Services.AddSingleton<IStorageBackend, MinioStorageService>();

// إضافة Swagger للتوثيق التفاعلي للـ API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// تكوين سير عمل طلبات HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();




