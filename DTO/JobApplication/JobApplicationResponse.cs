using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.DTO.JobApplication
{
    public class JobApplicationResponse
    {
        public int JobApplicationId { get; set; }

        public int StudentId { get; set; }

        public string RegisterNumber { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public int JobOpeningId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? CoverLetter { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime AppliedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }
    }
}