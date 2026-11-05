using CampusPlacementManagementPlatform.Data;
using CampusPlacementManagementPlatform.Models;
using CampusPlacementManagementPlatform.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagementPlatform.Respositories.Implementations
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> CreateAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.CompanyName)
                .ToListAsync();
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == id);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
    }
}