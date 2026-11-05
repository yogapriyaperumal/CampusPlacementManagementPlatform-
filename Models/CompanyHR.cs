using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class CompanyHR
    {
        public int CompanyHRId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        [MaxLength(100)]
        public string? Designation { get; set; }

        public CompanyHRStatus Status { get; set; }
            = CompanyHRStatus.Pending;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}