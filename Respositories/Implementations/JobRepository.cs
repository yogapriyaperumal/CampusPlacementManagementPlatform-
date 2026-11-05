using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<JobOpening> JobQuery()
        {
            return _context.JobOpenings
                .Include(j => j.Company)
                .Include(j => j.CreatedByCompanyHR)
                .Include(j => j.TargetColleges)
                    .ThenInclude(t => t.College);
        }

        public async Task<JobOpening> CreateAsync(JobOpening job)
        {
            await _context.JobOpenings.AddAsync(job);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(job.JobOpeningId) ?? job;
        }

        public async Task<List<JobOpening>> GetAllAsync()
        {
            return await JobQuery()
                .AsNoTracking()
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<JobOpening?> GetByIdAsync(int id)
        {
            return await JobQuery()
                .FirstOrDefaultAsync(j => j.JobOpeningId == id);
        }

        public async Task<List<JobOpening>> GetByCompanyHRAsync(
            int companyHRId)
        {
            return await JobQuery()
                .Where(j => j.CreatedByCompanyHRId == companyHRId)
                .AsNoTracking()
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<JobOpening>> GetByCollegeAsync(
            int collegeId)
        {
            return await JobQuery()
                .Where(j => j.Status == JobStatus.Published &&
                    j.TargetColleges.Any(t => t.CollegeId == collegeId))
                .AsNoTracking()
                .OrderByDescending(j => j.PublishedAt)
                .ToListAsync();
        }

        public async Task<JobOpening> UpdateAsync(JobOpening job)
        {
            await _context.SaveChangesAsync();

            return await GetByIdAsync(job.JobOpeningId) ?? job;
        }
    }
}