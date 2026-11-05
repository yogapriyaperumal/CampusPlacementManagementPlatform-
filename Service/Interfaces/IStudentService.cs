using CampusPlacementManagementPlatform.DTO.Student;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponse> CreateAsync(
            int officerId,
            StudentCreateRequest request);

        Task<List<StudentResponse>> GetAllAsync();

        Task<StudentResponse?> GetByIdAsync(int id);

        Task<List<StudentResponse>> GetByOfficerIdAsync(int officerId);

        Task<StudentResponse> UpdateAsync(
            int id,
            int officerId,
            StudentUpdateRequest request);

        Task<StudentResponse> ApproveAsync(int id, int officerId);

        Task<StudentResponse> RejectAsync(int id, int officerId);

        Task<StudentImportResult> ImportExcelAsync(
            int officerId,
            IFormFile file);
    }
}