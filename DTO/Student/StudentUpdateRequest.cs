using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.DTO.Student
{
    public class StudentUpdateRequest
    {
        [Required]
        public string Department { get; set; } = string.Empty;

        [Required]
        public string Degree { get; set; } = string.Empty;

        [Range(2000, 2100)]
        public int GraduationYear { get; set; }

        [Range(typeof(decimal), "0", "10")]
        public decimal CGPA { get; set; }

        [Range(0, 100)]
        public int ActiveBacklogs { get; set; }

        public string? ResumeUrl { get; set; }
    }
}