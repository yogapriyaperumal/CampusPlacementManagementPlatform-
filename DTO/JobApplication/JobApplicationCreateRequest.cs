using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.JobApplication
{
    public class JobApplicationCreateRequest
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int JobOpeningId { get; set; }

        [MaxLength(2000)]
        public string? CoverLetter { get; set; }
    }
}