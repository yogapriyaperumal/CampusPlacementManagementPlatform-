using System.ComponentModel.DataAnnotations;
using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Models
{
    public class InterviewRound
    {
        public int InterviewRoundId { get; set; }

        public int RecruitmentDriveId { get; set; }

        public RecruitmentDrive RecruitmentDrive { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string RoundName { get; set; } = string.Empty;

        public int RoundNumber { get; set; }

        public InterviewRoundType RoundType { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        public InterviewRoundStatus Status { get; set; }
            = InterviewRoundStatus.Planned;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}