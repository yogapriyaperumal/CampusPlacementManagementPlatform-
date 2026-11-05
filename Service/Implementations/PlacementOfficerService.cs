using CampusPlacementManagementPlatform.DTO.PlacementOfficer;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class PlacementOfficerService : IPlacementOfficerService
    {
        private readonly IPlacementOfficerRepository _officerRepository;
        private readonly ICollegeRepository _collegeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlacementOfficerService(
            IPlacementOfficerRepository officerRepository,
            ICollegeRepository collegeRepository,
            UserManager<ApplicationUser> userManager)
        {
            _officerRepository = officerRepository;
            _collegeRepository = collegeRepository;
            _userManager = userManager;
        }

        // Register a placement officer
        public async Task<PlacementOfficerResponse> CreateAsync(
            PlacementOfficerCreateRequest request)
        {
            // Check whether the Identity user exists
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Identity user does not exist.");
            }

            // Prevent duplicate officer profiles
            var existingOfficer =
                await _officerRepository.GetByUserIdAsync(request.UserId);

            if (existingOfficer != null)
            {
                throw new InvalidOperationException(
                    "This user is already registered as a placement officer.");
            }

            // Check whether the college exists
            var college =
                await _collegeRepository.GetByIdAsync(request.CollegeId);

            if (college == null)
            {
                throw new InvalidOperationException(
                    "College does not exist.");
            }

            // Only approved colleges can register placement officers
            if (college.Status != CollegeStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The college must be approved before registering a placement officer.");
            }

            var officer = new PlacementOfficer
            {
                UserId = request.UserId,
                CollegeId = request.CollegeId,
                Designation = request.Designation,
                Status = OfficerStatus.Pending
            };

            var createdOfficer =
                await _officerRepository.CreateAsync(officer);

            return MapToResponse(createdOfficer);
        }

        // Retrieve all placement officers
        public async Task<List<PlacementOfficerResponse>> GetAllAsync()
        {
            var officers = await _officerRepository.GetAllAsync();

            return officers.Select(MapToResponse).ToList();
        }

        // Retrieve placement officer by ID
        public async Task<PlacementOfficerResponse?> GetByIdAsync(int id)
        {
            var officer = await _officerRepository.GetByIdAsync(id);

            return officer == null ? null : MapToResponse(officer);
        }

        // Update placement officer designation
        public async Task<PlacementOfficerResponse?> UpdateAsync(
            int id,
            PlacementOfficerUpdateRequest request)
        {
            var officer = await _officerRepository.GetByIdAsync(id);

            if (officer == null)
            {
                return null;
            }

            officer.Designation = request.Designation;

            await _officerRepository.UpdateAsync(officer);

            return MapToResponse(officer);
        }

        // Approve a placement officer
        public async Task<PlacementOfficerResponse?> ApproveAsync(
            int id,
            string approvedBy)
        {
            var officer = await _officerRepository.GetByIdAsync(id);

            if (officer == null)
            {
                return null;
            }

            if (officer.Status != OfficerStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending placement officers can be approved.");
            }

            var approver = await _userManager.FindByIdAsync(approvedBy);

            if (approver == null)
            {
                throw new InvalidOperationException(
                    "Approving user does not exist.");
            }

            officer.Status = OfficerStatus.Approved;
            officer.ApprovedBy = approvedBy;
            officer.ApprovedAt = DateTime.UtcNow;

            await _officerRepository.UpdateAsync(officer);

            return MapToResponse(officer);
        }

        // Reject a placement officer
        public async Task<PlacementOfficerResponse?> RejectAsync(int id)
        {
            var officer = await _officerRepository.GetByIdAsync(id);

            if (officer == null)
            {
                return null;
            }

            if (officer.Status != OfficerStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending placement officers can be rejected.");
            }

            officer.Status = OfficerStatus.Rejected;
            officer.ApprovedBy = null;
            officer.ApprovedAt = null;

            await _officerRepository.UpdateAsync(officer);

            return MapToResponse(officer);
        }

        // Convert entity into response DTO
        private static PlacementOfficerResponse MapToResponse(
            PlacementOfficer officer)
        {
            return new PlacementOfficerResponse
            {
                PlacementOfficerId = officer.PlacementOfficerId,
                UserId = officer.UserId,
                CollegeId = officer.CollegeId,
                CollegeName = officer.College.CollegeName,
                Designation = officer.Designation,
                Status = officer.Status.ToString(),
                ApprovedBy = officer.ApprovedBy,
                ApprovedAt = officer.ApprovedAt
            };
        }
    }
}