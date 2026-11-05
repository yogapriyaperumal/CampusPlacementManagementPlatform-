using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class CompanyHRRepository : ICompanyHRRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyHRRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create a new CompanyHR profile
        public async Task<CompanyHR> CreateAsync(CompanyHR companyHR)
        {
            await _context.CompanyHRs.AddAsync(companyHR);

            await _context.SaveChangesAsync();

            return await _context.CompanyHRs
                .Include(h => h.Company)
                .FirstAsync(h => h.CompanyHRId == companyHR.CompanyHRId);
        }

        // Retrieve all CompanyHR records
        public async Task<List<CompanyHR>> GetAllAsync()
        {
            return await _context.CompanyHRs
                .Include(h => h.Company)
                .AsNoTracking()
                .OrderBy(h => h.CompanyHRId)
                .ToListAsync();
        }

        // Retrieve CompanyHR by ID
        public async Task<CompanyHR?> GetByIdAsync(int id)
        {
            return await _context.CompanyHRs
                .Include(h => h.Company)
                .FirstOrDefaultAsync(h => h.CompanyHRId == id);
        }

        // Retrieve CompanyHR using Identity User ID
        public async Task<CompanyHR?> GetByUserIdAsync(string userId)
        {
            return await _context.CompanyHRs
                .Include(h => h.Company)
                .FirstOrDefaultAsync(h => h.UserId == userId);
        }

        // Update CompanyHR information
        public async Task UpdateAsync(CompanyHR companyHR)
        {
            _context.CompanyHRs.Update(companyHR);

            await _context.SaveChangesAsync();
        }
    }
}