namespace CampusPlacementManagementPlatform.DTO.CompanyHR
{
    public class CompanyHRResponse
    {
        public int CompanyHRId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? Designation { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}