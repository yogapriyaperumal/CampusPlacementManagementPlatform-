using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Respositories.Implementations;
using CampusPlacementManagementPlatform.Service.Interfaces;
using CampusPlacementManagementPlatform.Service.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 1. Read database connection string
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Register Entity Framework Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Register ASP.NET Core Identity
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager();
// College repository registration
builder.Services.AddScoped<ICollegeRepository, CollegeRepository>();

// College service registration
builder.Services.AddScoped<ICollegeService, CollegeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();