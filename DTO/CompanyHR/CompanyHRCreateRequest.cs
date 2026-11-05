using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.CompanyHR
{
    public class CompanyHRCreateRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(100)]
        public string? Designation { get; set; }
    }
}