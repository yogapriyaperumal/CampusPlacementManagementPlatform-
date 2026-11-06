namespace CampusPlacementManagementPlatform.DTO.RecruitmentDrive
{
    public class RecruitmentDriveResponse
    {
        public int RecruitmentDriveId { get; set; }

        public string DriveTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public int CompanyHRId { get; set; }

        public int JobOpeningId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Mode { get; set; } = string.Empty;

        public string? VenueOrMeetingLink { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}