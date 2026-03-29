using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCIApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MCIApi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/{lang:regex(^(en|ar)$)}/[controller]")]
    public class LocationController : BaseApiController
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("governments")]
        public async Task<IActionResult> GetGovernments(string lang, CancellationToken cancellationToken = default)
        {
            var isArabic = lang == "ar";

            var governments = await _context.Governments
                .AsNoTracking()
                .Where(g => !g.IsDeleted)
                .OrderBy(g => isArabic ? g.NameAr : g.NameEn)
                .Select(g => new
                {
                    g.Id,
                    Name = isArabic ? g.NameAr : g.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(governments);
        }

        [HttpGet("governments/{governmentId:int}/cities")]
        public async Task<IActionResult> GetCities(string lang, int governmentId, CancellationToken cancellationToken = default)
        {
            var exists = await _context.Governments
                .AsNoTracking()
                .AnyAsync(g => g.Id == governmentId && !g.IsDeleted, cancellationToken);

            if (!exists)
                return NotFound(new { message = "Government not found" });

            var isArabic = lang == "ar";

            var cities = await _context.Cities
                .AsNoTracking()
                .Where(c => c.GovernmentId == governmentId && !c.IsDeleted)
                .OrderBy(c => isArabic ? c.NameAr : c.NameEn)
                .Select(c => new
                {
                    c.Id,
                    Name = isArabic ? c.NameAr : c.NameEn
                })
                .ToListAsync(cancellationToken);

            return Ok(cities);
        }
    }
}


