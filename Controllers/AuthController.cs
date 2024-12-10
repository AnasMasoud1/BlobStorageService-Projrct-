//namespace BlobStorageService.Controllers
//{
//    public class AuthController
//    {
//    }
//}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Text;
//using System;

//namespace BlobStorageService.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private const string SecretKey = "Anas1234"; // المفتاح السري الذي سيتم استخدامه لتوقيع التوكن

//        // نقطة النهاية لتوليد التوكن
//        [HttpPost("generate-token")]
//        public IActionResult GenerateToken()
//        {
//            var claims = new[]
//            {
//                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "Anas"),
//                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user123"),
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)); // المفتاح السري
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//        issuer: "BlobStorageAPI",  // اسم الخدمة التي تصدر التوكن
//        audience: "BlobStorageApp", // اسم التطبيق الذي سيستخدم التوكن
//        claims: claims,
//        expires: DateTime.Now.AddHours(1),  // جعل مدة صلاحية التوكن ساعة واحدة
//        signingCredentials: creds
//    );


//            var tokenString = new JwtSecurityTokenHandler().WriteToken(token); // توليد التوكن كـ string

//            return Ok(new { token = tokenString });  // إرجاع التوكن في استجابة JSON
//        }
//    }
//}



//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Text;
//using System;

//namespace BlobStorageService.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private const string SecretKey = "Anas1234!@#StrongSecretKey12345678"; // المفتاح السري

//        public class LoginRequest
//        {
//            public string Username { get; set; }
//            public string Password { get; set; }
//        }

//        [HttpPost("generate-token")]
//        public IActionResult GenerateToken([FromBody] LoginRequest loginRequest)
//        {
//            try
//            {
//                // تحقق من بيانات المستخدم
//                if (loginRequest.Username != "Anas" || loginRequest.Password != "Password123")
//                {
//                    return Unauthorized();
//                }

//                var claims = new[]
//                {
//                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, loginRequest.Username),
//                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user123"),
//                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin"),
//                };

//                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
//                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//                var token = new JwtSecurityToken(
//                    issuer: "BlobStorageAPI",
//                    audience: "BlobStorageApp",
//                    claims: claims,
//                    expires: DateTime.Now.AddHours(1),
//                    signingCredentials: creds
//                );

//                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

//                return Ok(new { token = tokenString });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = "An error occurred while generating the token", details = ex.Message });
//            }
//        }
//    }
//}




using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BlobStorageService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const string SecretKey = "Anas1234!@#StrongSecretKey123456789012";

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // نقطة النهاية لتوليد التوكن
        [HttpPost("generate-token")]
        public IActionResult GenerateToken([FromBody] LoginRequest loginRequest)
        {
            // تحقق من صحة بيانات المستخدم (يمكنك استخدام قاعدة بيانات أو منطق تحقق آخر)
            if (loginRequest.Username != "Anas" || loginRequest.Password != "Password123")
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, loginRequest.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "user123"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "BlobStorageAPI",
                audience: "BlobStorageApp",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // إضافة التوكن إلى Response Header
            Response.Headers.Add("Authorization", $"Bearer {tokenString}");

            // إرجاع التوكن في الـ Body كاختياري
            return Ok(new
            {
                token = tokenString,
                expiresIn = 3600 // مدة انتهاء التوكن بالثواني
            });
        }
    }
}
