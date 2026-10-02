using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PortfolioWebsite.Data;
using PortfolioWebsite.Models.Entities;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database - SQLite (lightweight, no SQL Server needed)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    // Seed default admin user if none exists
    if (!context.AdminUsers.Any())
    {
        context.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            Email = "admin@portfolio.com",
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();
    }

    // Seed default profile if none exists
    if (!context.Profiles.Any())
    {
        context.Profiles.Add(new Profile
        {
            FullName = "Your Name",
            Title = "Full Stack .NET Developer",
            Tagline = "Building amazing web applications",
            Bio = "I am a passionate full-stack developer with expertise in ASP.NET Core, C#, and modern web technologies. I love building scalable, high-performance applications.",
            Email = "hello@yourdomain.com",
            Phone = "+1 234 567 890",
            Location = "Your City, Country",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        context.SaveChanges();
    }
}

app.Run();

static string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}
