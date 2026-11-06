using CampusPlacementManagementPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<College> Colleges { get; set; }

        public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyHR> CompanyHRs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<JobOpening> JobOpenings { get; set; }

        public DbSet<JobTargetCollege> JobTargetColleges { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories
        { get; set; }
        public DbSet<RecruitmentDrive> RecruitmentDrives { get; set; }
        public DbSet<InterviewRound> InterviewRounds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<College>(entity =>
            {
                entity.HasKey(c => c.CollegeId);

                entity.HasIndex(c => c.CollegeCode)
                      .IsUnique();

                entity.Property(c => c.CollegeName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.CollegeCode)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.OfficialEmail)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(c => c.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(c => c.Address)
                      .IsRequired()
                      .HasMaxLength(500);
            });
            builder.Entity<PlacementOfficer>(entity =>
            {
                entity.HasKey(p => p.PlacementOfficerId);

                // One Identity user can have only one officer profile.
                entity.HasOne(p => p.User)
                      .WithOne()
                      .HasForeignKey<PlacementOfficer>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => p.UserId)
                      .IsUnique();

                // One college can have multiple placement officers.
                entity.HasOne(p => p.College)
                      .WithMany()
                      .HasForeignKey(p => p.CollegeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Stores the SuperAdmin who approved the officer.
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(p => p.ApprovedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.CompanyId);

                entity.Property(c => c.CompanyName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.CompanyEmail)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(c => c.Industry)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Address)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(c => c.Description)
                      .HasMaxLength(1000);
            });
            builder.Entity<CompanyHR>(entity =>
            {
                entity.HasKey(h => h.CompanyHRId);

                // One Identity user can have one CompanyHR profile.
                entity.HasOne(h => h.User)
                      .WithOne()
                      .HasForeignKey<CompanyHR>(h => h.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(h => h.UserId)
                      .IsUnique();

                // One company can have multiple HR representatives.
                entity.HasOne(h => h.Company)
                      .WithMany()
                      .HasForeignKey(h => h.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                // References the Identity user who approved the HR.
                entity.HasOne<ApplicationUser>()
                      .WithMany()
                      .HasForeignKey(h => h.ApprovedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(h => h.Designation)
                      .HasMaxLength(100);
            });
            builder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                // One Identity user can have only one student profile.
                entity.HasOne(s => s.User)
                    .WithOne()
                    .HasForeignKey<Student>(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(s => s.UserId)
                    .IsUnique();

                // One college can have multiple students.
                entity.HasOne(s => s.College)
                    .WithMany()
                    .HasForeignKey(s => s.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);

                // One placement officer manages multiple students.
                entity.HasOne(s => s.PlacementOfficer)
                    .WithMany()
                    .HasForeignKey(s => s.PlacementOfficerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Register number must be unique within a college.
                entity.HasIndex(s => new
                {
                    s.CollegeId,
                    s.RegisterNumber
                }).IsUnique();

                entity.Property(s => s.CGPA)
                    .HasPrecision(4, 2);

                entity.Property(s => s.RegisterNumber)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(s => s.Department)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(s => s.Degree)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(s => s.Status)
                    .HasConversion<int>();
            });
            // JobOpening configuration
            builder.Entity<JobOpening>(entity =>
            {
                entity.HasKey(j => j.JobOpeningId);

                entity.Property(j => j.JobTitle)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(j => j.JobDescription)
                    .HasMaxLength(3000)
                    .IsRequired();

                entity.Property(j => j.Location)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(j => j.EligibleDepartments)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(j => j.SalaryMin)
                    .HasPrecision(12, 2);

                entity.Property(j => j.SalaryMax)
                    .HasPrecision(12, 2);

                entity.Property(j => j.MinimumCGPA)
                    .HasPrecision(4, 2);

                entity.Property(j => j.Status)
                    .HasConversion<int>();

                entity.Property(j => j.EmploymentType)
                    .HasConversion<int>();

                entity.HasOne(j => j.Company)
                    .WithMany()
                    .HasForeignKey(j => j.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(j => j.CreatedByCompanyHR)
                    .WithMany()
                    .HasForeignKey(j => j.CreatedByCompanyHRId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // JobTargetCollege configuration
            builder.Entity<JobTargetCollege>(entity =>
            {
                entity.HasKey(j => j.JobTargetCollegeId);

                entity.HasOne(j => j.JobOpening)
                    .WithMany(j => j.TargetColleges)
                    .HasForeignKey(j => j.JobOpeningId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(j => j.College)
                    .WithMany()
                    .HasForeignKey(j => j.CollegeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(j => new
                {
                    j.JobOpeningId,
                    j.CollegeId
                }).IsUnique();
            });
            // JobApplication configuration
            builder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(a => a.JobApplicationId);

                entity.HasOne(a => a.Student)
                    .WithMany()
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.JobOpening)
                    .WithMany()
                    .HasForeignKey(a => a.JobOpeningId)
                    .OnDelete(DeleteBehavior.Restrict);

                // A student can apply only once for the same job.
                entity.HasIndex(a => new
                {
                    a.StudentId,
                    a.JobOpeningId
                }).IsUnique();

                entity.Property(a => a.Status)
                    .HasConversion<int>();

                entity.Property(a => a.CoverLetter)
                    .HasMaxLength(2000);
            });

            // ApplicationStatusHistory configuration
            builder.Entity<ApplicationStatusHistory>(entity =>
            {
                entity.HasKey(h => h.ApplicationStatusHistoryId);

                entity.HasOne(h => h.JobApplication)
                    .WithMany(a => a.StatusHistory)
                    .HasForeignKey(h => h.JobApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(h => h.PreviousStatus)
                    .HasConversion<int>();

                entity.Property(h => h.NewStatus)
                    .HasConversion<int>();

                entity.Property(h => h.Remarks)
                    .HasMaxLength(1000);

                entity.Property(h => h.UpdatedByUserId)
                    .HasMaxLength(450);
            });
            builder.Entity<RecruitmentDrive>(entity =>
            {
                entity.HasKey(rd => rd.RecruitmentDriveId);

                entity.Property(rd => rd.DriveTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(rd => rd.Description)
                    .HasMaxLength(1000);

                entity.Property(rd => rd.Mode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(rd => rd.VenueOrMeetingLink)
                    .HasMaxLength(500);

                entity.Property(rd => rd.Status)
                    .HasConversion<int>();

                entity.HasOne(rd => rd.Company)
                    .WithMany()
                    .HasForeignKey(rd => rd.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rd => rd.CompanyHR)
                    .WithMany()
                    .HasForeignKey(rd => rd.CompanyHRId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(rd => rd.JobOpening)
                    .WithMany()
                    .HasForeignKey(rd => rd.JobOpeningId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(rd => rd.JobOpeningId);
                entity.HasIndex(rd => rd.CompanyId);
            });
            builder.Entity<InterviewRound>(entity =>
            {
                entity.HasKey(ir => ir.InterviewRoundId);

                entity.Property(ir => ir.RoundName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(ir => ir.Description)
                    .HasMaxLength(1000);

                entity.Property(ir => ir.RoundType)
                    .HasConversion<int>();

                entity.Property(ir => ir.Status)
                    .HasConversion<int>();

                entity.Property(ir => ir.RoundNumber)
                    .IsRequired();

                entity.Property(ir => ir.DurationMinutes)
                    .IsRequired();

                entity.HasOne(ir => ir.RecruitmentDrive)
                    .WithMany()
                    .HasForeignKey(ir => ir.RecruitmentDriveId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ir => new
                {
                    ir.RecruitmentDriveId,
                    ir.RoundNumber
                }).IsUnique();
            });
        }
    }
}