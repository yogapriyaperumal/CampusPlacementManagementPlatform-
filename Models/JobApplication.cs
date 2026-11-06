using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class JobApplication
    {
        public int JobApplicationId { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        public int JobOpeningId { get; set; }

        public JobOpening JobOpening { get; set; } = null!;

        [MaxLength(2000)]
        public string? CoverLetter { get; set; }

        public ApplicationStatus Status { get; set; }
            = ApplicationStatus.Applied;

        public DateTime AppliedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? LastUpdatedAt { get; set; }

        public ICollection<ApplicationStatusHistory> StatusHistory
        { get; set; } = new List<ApplicationStatusHistory>();
    }
}