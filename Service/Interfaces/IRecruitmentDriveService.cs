using CampusPlacementManagementPlatform.DTO.RecruitmentDrive;

namespace CampusPlacementManagementPlatform.Service.Interfaces
{
    public interface IRecruitmentDriveService
    {
        Task<RecruitmentDriveResponse> CreateAsync(
            RecruitmentDriveCreateRequest request);

        Task<List<RecruitmentDriveResponse>> GetAllAsync();

        Task<RecruitmentDriveResponse> GetByIdAsync(int id);

        Task<List<RecruitmentDriveResponse>> GetByCompanyAsync(int companyId);

        Task<RecruitmentDriveResponse> UpdateAsync(
            int id,
            RecruitmentDriveUpdateRequest request);

        Task<RecruitmentDriveResponse> UpdateStatusAsync(
            int id,
            RecruitmentDriveStatusUpdateRequest request);
    }
}