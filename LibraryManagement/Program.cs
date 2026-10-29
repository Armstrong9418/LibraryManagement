
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Enregistrement des services MVC 
            builder.Services.AddControllersWithViews();

            // Enregistrement du DbContext avec SQL Server 
            builder.Services.AddDbContext<LibraryContext>(options =>
                options.UseSqlServer(

            builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            // Route par défaut → BooksController > Index 
            app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Books}/{action=Index}/{id?}");
            app.Run();
        }
    }
}