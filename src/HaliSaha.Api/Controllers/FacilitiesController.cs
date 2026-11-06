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
        public async Task<ActionResult<IEnumerable<Facility>>> GetApprovedFacilities()
        {
            var approvedFacilities = await _context.Facilities
                .Where(f => f.Status == FacilityStatus.Approved).ToListAsync();

            return Ok(approvedFacilities);
        }
    }
}
