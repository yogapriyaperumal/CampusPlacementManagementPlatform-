using CampusPlacementManagementPlatform.DTO.College;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class CollegeService : ICollegeService
    {
        private readonly ICollegeRepository _collegeRepository;

        public CollegeService(ICollegeRepository collegeRepository)
        {
            _collegeRepository = collegeRepository;
        }

        public async Task<CollegeResponse> CreateAsync(
            CollegeCreateRequest request)
        {
            // Normalize the college code
            string collegeCode = request.CollegeCode.Trim().ToUpperInvariant();

            // Check whether the college code already exists
            var existingCollege =
                await _collegeRepository.GetByCodeAsync(collegeCode);

            if (existingCollege != null)
            {
                throw new InvalidOperationException(
                    "A college with this code already exists.");
            }

            // Convert request DTO into entity
            var college = new College
            {
                CollegeName = request.CollegeName.Trim(),
                CollegeCode = collegeCode,
                UniversityName = request.UniversityName?.Trim(),
                OfficialEmail = request.OfficialEmail.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address.Trim(),
                Website = request.Website?.Trim(),

                // New colleges must start as Pending
                Status = CollegeStatus.Pending,

                CreatedAt = DateTime.UtcNow
            };

            var createdCollege =
                await _collegeRepository.CreateAsync(college);

            return MapToResponse(createdCollege);
        }

        public async Task<List<CollegeResponse>> GetAllAsync()
        {
            var colleges = await _collegeRepository.GetAllAsync();

            return colleges.Select(MapToResponse).ToList();
        }

        public async Task<CollegeResponse?> GetByIdAsync(int id)
        {
            var college = await _collegeRepository.GetByIdAsync(id);

            if (college == null)
                return null;

            return MapToResponse(college);
        }

        public async Task<CollegeResponse?> UpdateAsync(
            int id,
            CollegeUpdateRequest request)
        {
            var college = await _collegeRepository.GetByIdAsync(id);

            if (college == null)
                return null;

            college.CollegeName = request.CollegeName.Trim();
            college.UniversityName = request.UniversityName?.Trim();
            college.OfficialEmail = request.OfficialEmail.Trim();
            college.PhoneNumber = request.PhoneNumber.Trim();
            college.Address = request.Address.Trim();
            college.Website = request.Website?.Trim();

            await _collegeRepository.UpdateAsync(college);

            return MapToResponse(college);
        }

        // Converts database entity into response DTO
        private static CollegeResponse MapToResponse(College college)
        {
            return new CollegeResponse
            {
                CollegeId = college.CollegeId,
                CollegeName = college.CollegeName,
                CollegeCode = college.CollegeCode,
                UniversityName = college.UniversityName,
                OfficialEmail = college.OfficialEmail,
                PhoneNumber = college.PhoneNumber,
                Address = college.Address,
                Website = college.Website,
                Status = college.Status.ToString(),
                CreatedAt = college.CreatedAt
            };
        }
    }
}