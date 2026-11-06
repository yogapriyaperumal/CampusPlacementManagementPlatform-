using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class ApplicationStatusHistory
    {
        public int ApplicationStatusHistoryId { get; set; }

        public int JobApplicationId { get; set; }

        public JobApplication JobApplication { get; set; } = null!;

        public ApplicationStatus PreviousStatus { get; set; }

        public ApplicationStatus NewStatus { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public string? UpdatedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }
            = DateTime.UtcNow;
    }
}