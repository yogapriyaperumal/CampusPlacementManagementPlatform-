namespace CampusPlacementManagementPlatform.DTO.JobApplication
{
    public class ApplicationStatusHistoryResponse
    {
        public int ApplicationStatusHistoryId { get; set; }

        public string PreviousStatus { get; set; } = string.Empty;

        public string NewStatus { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public string? UpdatedByUserId { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}