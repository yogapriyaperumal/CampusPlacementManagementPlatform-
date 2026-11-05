using System.ComponentModel.DataAnnotations;
using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.DTO.Job
{
    public class JobCreateRequest
    {
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

        [Range(typeof(decimal), "0", "10")]
        public decimal MinimumCGPA { get; set; }

        [Range(0, 100)]
        public int MaximumBacklogs { get; set; }

        [Range(2000, 2100)]
        public int EligibleGraduationYear { get; set; }

        [Required]
        public string EligibleDepartments { get; set; } = string.Empty;

        public DateTime ApplicationDeadline { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> TargetCollegeIds { get; set; } = new();
    }
}