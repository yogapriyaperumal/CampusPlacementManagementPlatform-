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
        }
    }
}