using Microsoft.EntityFrameworkCore;
using WeatherViewer.Data;
using WeatherViewer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<SessionService>();
builder.Services.AddHostedService<SessionHostedService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// add middleware
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();