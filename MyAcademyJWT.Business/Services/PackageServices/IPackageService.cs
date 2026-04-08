using MyAcademyJWT.Business.DTOs.PackageDtos;

namespace MyAcademyJWT.Business.Services.PackageServices
{
    public interface IPackageService
    {
        Task<List<ResultPackageDto>> GetAllPackagesAsync();
    }
}
