using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class RecruitmentDriveRepository : IRecruitmentDriveRepository
    {
        private readonly ApplicationDbContext _context;

        public RecruitmentDriveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<RecruitmentDrive> DriveQuery()
        {
            return _context.RecruitmentDrives
                .Include(d => d.Company)
                .Include(d => d.CompanyHR)
                .Include(d => d.JobOpening);
        }

        public async Task<RecruitmentDrive> CreateAsync(RecruitmentDrive drive)
        {
            _context.RecruitmentDrives.Add(drive);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(drive.RecruitmentDriveId)
                ?? throw new InvalidOperationException("Drive could not be retrieved.");
        }

        public async Task<List<RecruitmentDrive>> GetAllAsync()
        {
            return await DriveQuery()
                .AsNoTracking()
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<RecruitmentDrive?> GetByIdAsync(int id)
        {
            return await DriveQuery()
                .FirstOrDefaultAsync(d => d.RecruitmentDriveId == id);
        }

        public async Task<List<RecruitmentDrive>> GetByCompanyAsync(int companyId)
        {
            return await DriveQuery()
                .AsNoTracking()
                .Where(d => d.CompanyId == companyId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(RecruitmentDrive drive)
        {
            await _context.SaveChangesAsync();
        }
    }
}