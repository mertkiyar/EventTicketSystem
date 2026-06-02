using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using EventTicketSystem.Data;
using EventTicketSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=events.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EventTicketAuth";
        options.LoginPath = "/Home/Index";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

var app = builder.Build();

// Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Ensure the database is migrated on startup
    context.Database.Migrate();

    if (!context.Users.Any(u => u.Email == "admin@example.com"))
    {
        var adminUser = new User
        {
            FullName = "Admin",
            Email = "admin@example.com",
            PasswordHash = HashPassword("admin123"),
            IsAdmin = true
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
// this sample data added because in Railway website,database usually will be empty, so it will add this data.
    if (!context.Events.Any())
    {
        var sampleEvents = new List<Event>
        {
            new Event
            {
                Title = "The Phantom of the Opera",
                Description = "Experience the classic musical masterpiece live on stage. A story of love, obsession, and music.",
                Location = "Royal Opera House",
                Date = DateTime.Now.AddDays(15),
                Time = new TimeOnly(19, 30),
                ImageUrl = "/images/concert_banner.png",
                TicketCount = 120,
                Category = "Theater"
            },
            new Event
            {
                Title = "Coldplay Music of the Spheres Tour",
                Description = "Join Coldplay for an unforgettable night of music, lights, and energy at the stadium.",
                Location = "Wembley Stadium",
                Date = DateTime.Now.AddDays(30),
                Time = new TimeOnly(20, 0),
                ImageUrl = "/images/concert_banner.png",
                TicketCount = 250,
                Category = "Concert"
            },
            new Event
            {
                Title = "Interstellar - IMAX Special Screening",
                Description = "Watch Christopher Nolan's sci-fi epic on the largest IMAX screen with enhanced audio.",
                Location = "IMAX Cinema Center",
                Date = DateTime.Now.AddDays(5),
                Time = new TimeOnly(18, 0),
                ImageUrl = "/images/concert_banner.png",
                TicketCount = 80,
                Category = "Cinema"
            },
            new Event
            {
                Title = "Sziget Festival 2026",
                Description = "One of the largest music and cultural festivals in Europe. 7 days of non-stop fun and artists.",
                Location = "Obuda Island, Budapest",
                Date = DateTime.Now.AddDays(60),
                Time = new TimeOnly(14, 0),
                ImageUrl = "/images/concert_banner.png",
                TicketCount = 500,
                Category = "Festival"
            },
            new Event
            {
                Title = "Champions League Final 2026 Viewing Party",
                Description = "Watch the biggest football match of the year on a giant screen with food, drinks, and fans.",
                Location = "Sports Arena Lounge",
                Date = DateTime.Now.AddDays(25),
                Time = new TimeOnly(21, 45),
                ImageUrl = "/images/concert_banner.png",
                TicketCount = 150,
                Category = "Sports"
            }
        };
        context.Events.AddRange(sampleEvents);
        context.SaveChanges();
    }
}

string HashPassword(string password)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return System.Convert.ToBase64String(hashedBytes);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
