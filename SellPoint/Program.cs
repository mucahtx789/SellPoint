
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SellPoint.Hubs;
using SellPoint.Models;
using System.Text;


namespace SellPoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Kestrel yapılandırması (backend portu)
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5195); // Vue frontend'in erişeceği backend portu
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // DbContext'i servis olarak ekliyoruz
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();

            //SignalR
            builder.Services.AddSignalR();


            // CORS ayarları
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

            // JWT Authentication yapılandırması
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Geliştirme ortamında HTTPS gerekmez
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Issuer'ı konfigürasyon dosyasından alıyoruz
                        ValidAudience = builder.Configuration["Jwt:Audience"],  // Audience'ı konfigürasyon dosyasından alıyoruz
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  // Anahtarınızı konfigürasyon dosyasından alıyoruz
                    };
                });
            // Authorization yapılandırması
            builder.Services.AddAuthorization();

            // MemoryCache ve Rate Limiting servisleri  API Rate Limiting (API İstek Sınırlaması)
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


            // Authentication ve Authorization middleware'lerini doğru sırayla ekliyoruz
            app.UseAuthentication();  // Öncelikle Authentication
            app.UseAuthorization();   // Sonrasında Authorization


            app.MapControllers();

            //signalr hubs
            app.MapHub<CartHub>("/carthub");
            app.MapHub<ProductHub>("/producthub");


            app.Run();
        }
    }
}
