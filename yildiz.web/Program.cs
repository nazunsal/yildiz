using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using yildiz.business.Abstract;
using yildiz.business.Concrete;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Concrete;
using yildiz.DataAccess.Context;

namespace yildiz.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddSession();

            builder.Services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            builder.Services.AddScoped<
                IProductRepository,
                ProductRepository>();

            builder.Services.AddScoped<
                IProductService,
                ProductManager>();

            builder.Services.AddScoped<
                ICategoryRepository,
                CategoryRepository>();

            builder.Services.AddScoped<
                ICategoryService,
                CategoryManager>();

            builder.Services.AddScoped<
                IOrderRepository,
                OrderRepository>();

            builder.Services.AddScoped<
                IOrderService,
                OrderManager>();

            builder.Services.AddScoped<
                IUserRepository,
                UserRepository>();

            builder.Services.AddScoped<
                IUserService,
                UserManager>();

            builder.Services.AddScoped<
                IPasswordResetTokenRepository,
                PasswordResetTokenRepository>();

            builder.Services.AddScoped<
                IPasswordResetTokenService,
                PasswordResetTokenManager>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}