using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class PlacementOfficer
    {
        public int PlacementOfficerId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public int CollegeId { get; set; }

        public College College { get; set; } = null!;

        [MaxLength(100)]
        public string? Designation { get; set; }

        public OfficerStatus Status { get; set; }
            = OfficerStatus.Pending;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}