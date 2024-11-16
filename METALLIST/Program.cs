using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace METALLIST
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Путь к странице входа
                options.AccessDeniedPath = "/Account/AccessDenied"; // Путь к странице отказа в доступе
                                                                    
                options.ExpireTimeSpan = TimeSpan.FromMinutes(1); // Время жизни куки: 30 минут

                options.SlidingExpiration = true; // Включить продление куки
            });
            builder.Services.AddAuthorization();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Регистрация DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // Используйте вашу строку подключения

            var app = builder.Build();

            app.UseAuthentication(); // Подключение аутентификации
            app.UseAuthorization();  // Подключение авторизации

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
