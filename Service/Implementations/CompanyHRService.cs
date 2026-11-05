using CampusPlacementManagementPlatform.DTO.CompanyHR;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using CampusPlacementManagementPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagementPlatform.Service.Implementations
{
    public class CompanyHRService : ICompanyHRService
    {
        private readonly ICompanyHRRepository _companyHRRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompanyHRService(
            ICompanyHRRepository companyHRRepository,
            ICompanyRepository companyRepository,
            UserManager<ApplicationUser> userManager)
        {
            _companyHRRepository = companyHRRepository;
            _companyRepository = companyRepository;
            _userManager = userManager;
        }

        // Register a CompanyHR
        public async Task<CompanyHRResponse> CreateAsync(
            CompanyHRCreateRequest request)
        {
            // Verify Identity user
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Identity user does not exist.");
            }

            // Prevent duplicate HR profiles
            var existingHR =
                await _companyHRRepository.GetByUserIdAsync(request.UserId);

            if (existingHR != null)
            {
                throw new InvalidOperationException(
                    "This user is already registered as a CompanyHR.");
            }

            // Verify company
            var company =
                await _companyRepository.GetByIdAsync(request.CompanyId);

            if (company == null)
            {
                throw new InvalidOperationException(
                    "Company does not exist.");
            }

            // Only approved companies can register HR representatives
            if (company.Status != CompanyStatus.Approved)
            {
                throw new InvalidOperationException(
                    "The company must be approved before registering HR representatives.");
            }

            var companyHR = new CompanyHR
            {
                UserId = request.UserId,
                CompanyId = request.CompanyId,
                Designation = request.Designation,
                Status = CompanyHRStatus.Pending
            };

            var createdHR =
                await _companyHRRepository.CreateAsync(companyHR);

            return MapToResponse(createdHR);
        }

        // Retrieve all CompanyHR records
        public async Task<List<CompanyHRResponse>> GetAllAsync()
        {
            var records = await _companyHRRepository.GetAllAsync();

            return records.Select(MapToResponse).ToList();
        }

        // Retrieve CompanyHR by ID
        public async Task<CompanyHRResponse?> GetByIdAsync(int id)
        {
            var record = await _companyHRRepository.GetByIdAsync(id);

            return record == null ? null : MapToResponse(record);
        }

        // Update CompanyHR designation
        public async Task<CompanyHRResponse?> UpdateAsync(
            int id,
            CompanyHRUpdateRequest request)
        {
            var record = await _companyHRRepository.GetByIdAsync(id);

            if (record == null)
            {
                return null;
            }

            record.Designation = request.Designation;

            await _companyHRRepository.UpdateAsync(record);

            return MapToResponse(record);
        }

        // Approve CompanyHR
        public async Task<CompanyHRResponse?> ApproveAsync(
            int id,
            string approvedBy)
        {
            var record = await _companyHRRepository.GetByIdAsync(id);

            if (record == null)
            {
                return null;
            }

            if (record.Status != CompanyHRStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending CompanyHR registrations can be approved.");
            }

            var approver = await _userManager.FindByIdAsync(approvedBy);

            if (approver == null)
            {
                throw new InvalidOperationException(
                    "Approving user does not exist.");
            }

            record.Status = CompanyHRStatus.Approved;
            record.ApprovedBy = approvedBy;
            record.ApprovedAt = DateTime.UtcNow;

            await _companyHRRepository.UpdateAsync(record);

            return MapToResponse(record);
        }

        // Reject CompanyHR
        public async Task<CompanyHRResponse?> RejectAsync(int id)
        {
            var record = await _companyHRRepository.GetByIdAsync(id);

            if (record == null)
            {
                return null;
            }

            if (record.Status != CompanyHRStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending CompanyHR registrations can be rejected.");
            }

            record.Status = CompanyHRStatus.Rejected;
            record.ApprovedBy = null;
            record.ApprovedAt = null;

            await _companyHRRepository.UpdateAsync(record);

            return MapToResponse(record);
        }

        // Entity to response DTO mapping
        private static CompanyHRResponse MapToResponse(CompanyHR companyHR)
        {
            return new CompanyHRResponse
            {
                CompanyHRId = companyHR.CompanyHRId,
                UserId = companyHR.UserId,
                CompanyId = companyHR.CompanyId,
                CompanyName = companyHR.Company.CompanyName,
                Designation = companyHR.Designation,
                Status = companyHR.Status.ToString(),
                ApprovedBy = companyHR.ApprovedBy,
                ApprovedAt = companyHR.ApprovedAt
            };
        }
    }
}