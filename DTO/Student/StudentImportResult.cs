namespace CampusPlacementManagementPlatform.DTO.Student
{
    public class StudentImportResult
    {
        public int TotalRecords { get; set; }

        public int ImportedRecords { get; set; }

        public int FailedRecords { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}