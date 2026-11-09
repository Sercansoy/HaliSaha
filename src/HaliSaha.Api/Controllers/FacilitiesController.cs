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
                .Select(FacilityResponse.Projection)
                .ToListAsync();

            return Ok(facilities);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FacilityResponse>> GetFacility(int id)
        {
            var facility = await _context.Facilities
                .Where(f => f.Id == id && f.Status == FacilityStatus.Approved)
                .Select(FacilityResponse.Projection)
                .FirstOrDefaultAsync();
                    
            if(facility == null)
            {
                return NotFound();
            }
            return Ok(facility);
        }
    }
}
