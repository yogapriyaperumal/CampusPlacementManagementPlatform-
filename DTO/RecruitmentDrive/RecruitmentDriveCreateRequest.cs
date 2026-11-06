using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.RecruitmentDrive
{
    public class RecruitmentDriveCreateRequest
    {
        [Required]
        [MaxLength(200)]
        public string DriveTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int CompanyHRId { get; set; }

        [Required]
        public int JobOpeningId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Mode { get; set; } = "Online";

        public string? VenueOrMeetingLink { get; set; }
    }
}