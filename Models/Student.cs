using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagementPlatform.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public int CollegeId { get; set; }

        public College College { get; set; } = null!;

        public int PlacementOfficerId { get; set; }

        public PlacementOfficer PlacementOfficer { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string RegisterNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Degree { get; set; } = string.Empty;

        [Range(2000, 2100)]
        public int GraduationYear { get; set; }

        [Range(typeof(decimal), "0", "10")]
        public decimal CGPA { get; set; }

        [Range(0, 100)]
        public int ActiveBacklogs { get; set; }

        [MaxLength(500)]
        public string? ResumeUrl { get; set; }

        public StudentStatus Status { get; set; }
            = StudentStatus.Pending;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }
    }
}