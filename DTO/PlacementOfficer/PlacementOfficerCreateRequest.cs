using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.PlacementOfficer
{
    public class PlacementOfficerCreateRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CollegeId { get; set; }

        [MaxLength(100)]
        public string? Designation { get; set; }
    }
}