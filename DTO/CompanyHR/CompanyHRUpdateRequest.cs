using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.CompanyHR
{
    public class CompanyHRUpdateRequest
    {
        [MaxLength(100)]
        public string? Designation { get; set; }
    }
}