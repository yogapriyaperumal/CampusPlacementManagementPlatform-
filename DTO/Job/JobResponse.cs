using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.DTO.Job
{
    public class JobResponse
    {
        public int JobOpeningId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string JobDescription { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string EmploymentType { get; set; } = string.Empty;

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public decimal MinimumCGPA { get; set; }

        public int MaximumBacklogs { get; set; }

        public int EligibleGraduationYear { get; set; }

        public string EligibleDepartments { get; set; } = string.Empty;

        public DateTime ApplicationDeadline { get; set; }

        public string Status { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public int CreatedByCompanyHRId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public List<int> TargetCollegeIds { get; set; } = new();

        public List<string> TargetCollegeNames { get; set; } = new();
    }
}