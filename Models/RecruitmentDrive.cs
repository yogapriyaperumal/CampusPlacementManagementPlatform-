using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class RecruitmentDrive
    {
        public int RecruitmentDriveId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DriveTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int CompanyHRId { get; set; }
        public CompanyHR CompanyHR { get; set; } = null!;

        public int JobOpeningId { get; set; }
        public JobOpening JobOpening { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Mode { get; set; } = "Online";

        [MaxLength(500)]
        public string? VenueOrMeetingLink { get; set; }

        public RecruitmentDriveStatus Status { get; set; }
            = RecruitmentDriveStatus.Planned;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}