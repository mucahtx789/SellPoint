
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellPoint.Models;
using System.Text;


namespace SellPoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Kestrel yap�land�rmas� (backend portu)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5195); // Vue frontend'in eri�ece�i backend portu
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext'i servis olarak ekliyoruz
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();

            // CORS ayarlar�
             builder.Services.AddCors(options =>
             {
                 options.AddPolicy("AllowVueApp", policy =>
                 {
                     policy.WithOrigins("http://localhost:64961") //  Vue app adresi
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials(); // JWT, cookie vs. izin ver
                 });
             });

            // JWT Authentication yap�land�rmas�
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Geli�tirme ortam�nda HTTPS gerekmez
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Issuer'� konfig�rasyon dosyas�ndan al�yoruz
                        ValidAudience = builder.Configuration["Jwt:Audience"],  // Audience'� konfig�rasyon dosyas�ndan al�yoruz
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  // Anahtar�n�z� konfig�rasyon dosyas�ndan al�yoruz
                    };
                });
            // Authorization yap�land�rmas�
            builder.Services.AddAuthorization();

            // MemoryCache ve Rate Limiting servisleri  API Rate Limiting (API �stek S�n�rlamas�)
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

           

            var app = builder.Build();

            app.UseCors("AllowVueApp");

            // Rate Limiting middleware'i ekle
            app.UseIpRateLimiting();

            app.UseHttpsRedirection();


            // Authentication ve Authorization middleware'lerini do�ru s�rayla ekliyoruz
            app.UseAuthentication();  // �ncelikle Authentication
            app.UseAuthorization();   // Sonras�nda Authorization


            app.MapControllers();

            app.Run();
        }
    }
}
