namespace CampusPlacementManagementPlatform.Models
{
    public class JobTargetCollege
    {
        public int JobTargetCollegeId { get; set; }

        public int JobOpeningId { get; set; }

        public JobOpening JobOpening { get; set; } = null!;

        public int CollegeId { get; set; }

        public College College { get; set; } = null!;
    }
}