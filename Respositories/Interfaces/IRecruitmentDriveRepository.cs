using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IRecruitmentDriveRepository
    {
        Task<RecruitmentDrive> CreateAsync(RecruitmentDrive drive);

        Task<List<RecruitmentDrive>> GetAllAsync();

        Task<RecruitmentDrive?> GetByIdAsync(int id);

        Task<List<RecruitmentDrive>> GetByCompanyAsync(int companyId);

        Task UpdateAsync(RecruitmentDrive drive);
    }
}