using System.ComponentModel.DataAnnotations;
using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.DTO.JobApplication
{
    public class ApplicationStatusUpdateRequest
    {
        [Required]
        public ApplicationStatus NewStatus { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }
    }
}