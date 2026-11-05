namespace CampusPlacementManagementPlatform.DTO.PlacementOfficer
{
    public class PlacementOfficerResponse
    {
        public int PlacementOfficerId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int CollegeId { get; set; }

        public string CollegeName { get; set; } = string.Empty;

        public string? Designation { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}