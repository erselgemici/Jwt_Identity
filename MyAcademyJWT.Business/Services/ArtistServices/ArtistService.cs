using Microsoft.EntityFrameworkCore;
using MyAcademyJWT.Business.DTOs.ArtistDtos;
using MyAcademyJWT.DataAccess.Context;

namespace MyAcademyJWT.Business.Services.ArtistServices
{
    public class ArtistService : IArtistService
    {
        private readonly AppDbContext _context;

        public ArtistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResultArtistDto>> GetAllArtistsAsync()
        {
            return await _context.Artists
                .Select(a => new ResultArtistDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl
                }).ToListAsync();
        }

        public async Task<ResultArtistDto> GetArtistByIdAsync(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return null;

            return new ResultArtistDto
            {
                Id = artist.Id,
                Name = artist.Name,
                ImageUrl = artist.ImageUrl
            };
        }
    }
}
