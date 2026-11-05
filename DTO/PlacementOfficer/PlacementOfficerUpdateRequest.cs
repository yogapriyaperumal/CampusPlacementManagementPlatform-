using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.PlacementOfficer
{
    public class PlacementOfficerUpdateRequest
    {
        [MaxLength(100)]
        public string? Designation { get; set; }
    }
}