using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Implementations;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Implementations;
using CampusPlacementManagementPlatform.Service.Interfaces;
using CampusPlacementManagementPlatform.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddScoped<
    IPlacementOfficerRepository,
    PlacementOfficerRepository>();

builder.Services.AddScoped<
    IPlacementOfficerService,
    PlacementOfficerService>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<
    ICompanyHRRepository,
    CompanyHRRepository>();

builder.Services.AddScoped<
    ICompanyHRService,
    CompanyHRService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();

builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<
    IJobApplicationRepository,
    JobApplicationRepository>();

builder.Services.AddScoped<
    IJobApplicationService,
    JobApplicationService>();
builder.Services.AddScoped<
    IRecruitmentDriveRepository,
    RecruitmentDriveRepository>();

builder.Services.AddScoped<
    IRecruitmentDriveService,
    RecruitmentDriveService>();
builder.Services.AddScoped<
    IInterviewRoundRepository,
    InterviewRoundRepository>();

builder.Services.AddScoped<
    IInterviewRoundService,
    InterviewRoundService>();

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