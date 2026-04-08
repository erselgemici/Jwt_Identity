using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.Business.DTOs.PackageDtos;
using MyAcademyJWT.DataAccess.Context;

namespace MyAcademyJWT.Business.Services.PackageServices
{
    public class PackageService : IPackageService
    {
        private readonly AppDbContext _context;

        public PackageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResultPackageDto>> GetAllPackagesAsync()
        {
            var packages = await _context.Packages
                .OrderBy(p => p.Id)
                .Select(p => new ResultPackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ContentLevel = p.ContentLevel
                }).ToListAsync();

            return packages;
        }
    }
}
