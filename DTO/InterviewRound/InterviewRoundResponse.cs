namespace CampusPlacementManagementPlatform.DTO.InterviewRound
{
    public class InterviewRoundResponse
    {
        public int InterviewRoundId { get; set; }

        public int RecruitmentDriveId { get; set; }

        public string DriveTitle { get; set; } = string.Empty;

        public string RoundName { get; set; } = string.Empty;

        public int RoundNumber { get; set; }

        public string RoundType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}