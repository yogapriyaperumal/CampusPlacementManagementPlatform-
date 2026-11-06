using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.RecruitmentDrive
{
    public class RecruitmentDriveUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string DriveTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Mode { get; set; } = "Online";

        public string? VenueOrMeetingLink { get; set; }
    }
}