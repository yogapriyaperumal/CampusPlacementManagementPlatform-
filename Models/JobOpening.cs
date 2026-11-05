using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class JobOpening
    {
        public int JobOpeningId { get; set; }

        [Required]
        [MaxLength(200)]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string JobDescription { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        public EmploymentType EmploymentType { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public decimal MinimumCGPA { get; set; }

        public int MaximumBacklogs { get; set; }

        public int EligibleGraduationYear { get; set; }

        [Required]
        [MaxLength(1000)]
        public string EligibleDepartments { get; set; } = string.Empty;

        public DateTime ApplicationDeadline { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Draft;

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public int CreatedByCompanyHRId { get; set; }

        public CompanyHR CreatedByCompanyHR { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PublishedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public ICollection<JobTargetCollege> TargetColleges { get; set; }
            = new List<JobTargetCollege>();
    }
}