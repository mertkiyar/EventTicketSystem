using Microsoft.EntityFrameworkCore;
using EventTicketSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=events.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Events.Any())
    {
        context.Events.AddRange(
            new EventTicketSystem.Models.Event
            {
                Title = "Dedublüman",
                Description = "Experience an unforgettable night.",
                Location = "Istanbul",
                Date = DateTime.Now.AddDays(3),
                ImageUrl = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a",
                TicketCount = 5000,
                Category = "Concert"
            },
            new EventTicketSystem.Models.Event
            {
                Title = "Tech Conference 2026",
                Description = "Meet developers and tech leaders.",
                Location = "Ankara",
                Date = DateTime.Now.AddDays(4),
                ImageUrl = "https://images.unsplash.com/photo-1511578314322-379afb476865",
                TicketCount = 1500,
                Category = "Technology"
            },
            new EventTicketSystem.Models.Event
            {
                Title = "Rihanna 2026 - Antalya",
                Description = "Big Day Coming",
                Location = "Antalya",
                Date = DateTime.Now.AddDays(8),
                ImageUrl = "https://www.billboard.com/wp-content/uploads/2022/06/rihanna-musicares-billboard-1548.jpg?w=942&h=628&crop=1",
                TicketCount = 150000,
                Category = "Concert"
            }
        );
        context.SaveChanges();
    }
}
app.Run();
