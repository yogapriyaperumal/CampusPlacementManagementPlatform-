using System.ComponentModel.DataAnnotations;
using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.DTO.InterviewRound
{
    public class InterviewRoundCreateRequest
    {
        [Required]
        [MaxLength(150)]
        public string RoundName { get; set; } = string.Empty;

        [Range(1, 50)]
        public int RoundNumber { get; set; }

        public InterviewRoundType RoundType { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 600)]
        public int DurationMinutes { get; set; }
    }
}