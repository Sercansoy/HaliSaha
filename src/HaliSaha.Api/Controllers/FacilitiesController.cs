using HaliSaha.Api.Contracts;
using HaliSaha.Core.Entities;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Api.Controllers
{
    [ApiController]
    [Route("api/facilities")]
    public class FacilitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacilitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityResponse>>> GetApprovedFacilities()
        {
            var facilities = await _context.Facilities
                .Where(f => f.Status == FacilityStatus.Approved)
                .Select(f => new FacilityResponse(
                    f.Id,
                    f.Name,
                    f.City,
                    f.District,
                    f.Address,
                    f.Phone,
                    f.Pitches
                    .Where(p => p.IsActive)
                    .Select(p => new PitchResponse(p.Id, p.Name, p.IsIndoor))
                    .ToList()))
                .ToListAsync();

            return Ok(facilities);
        }
    }
}
