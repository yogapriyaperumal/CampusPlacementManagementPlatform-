using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.College
{
    public class CollegeUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string CollegeName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? UniversityName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string OfficialEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Website { get; set; }
    }
}