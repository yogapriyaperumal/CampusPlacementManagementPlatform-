using CampusPlacementManagementPlatform.Models;

namespace CampusPlacementManagementPlatform.Respositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> CreateAsync(Student student);

        Task<List<Student>> GetAllAsync();

        Task<Student?> GetByIdAsync(int id);

        Task<Student?> GetByUserIdAsync(string userId);

        Task<Student?> GetByCollegeAndRegisterNumberAsync(
            int collegeId,
            string registerNumber);

        Task<List<Student>> GetByOfficerIdAsync(int officerId);

        Task<Student> UpdateAsync(Student student);
    }
}