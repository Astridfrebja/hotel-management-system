using Microsoft.EntityFrameworkCore;
using SamletInfo.Controllers;
using SamletInfo.Data;

namespace Oblig4Azure
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddConsole();

            builder.Services.AddControllersWithViews()
                .AddApplicationPart(typeof(AccountController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            builder.Services.AddDbContext<HotelContext>(options =>
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("SamletInfo")));

            builder.Services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            var app = builder.Build();

            await InitializeDatabaseAsync(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            await app.RunAsync();
        }

        private static async Task InitializeDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HotelContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            for (var attempt = 1; attempt <= 30; attempt++)
            {
                try
                {
                    await db.Database.MigrateAsync();
                    DbInitializer.Seed(db);
                    logger.LogInformation("Database is ready.");
                    return;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Waiting for SQL Server (attempt {Attempt}/30).", attempt);
                    if (attempt == 30)
                    {
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}
