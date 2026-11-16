using HaliSaha.Core.Entities;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HaliSaha.Api.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Api.Controllers
{
    [ApiController]
    [Route("api/admin/facilities")]
    [Authorize(Roles =nameof(UserRole.Admin))]
    public class AdminFacilitiesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminFacilitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<PendingFacilityResponse>>> GetPendingFacilities()
        {
            var pendingFacilities = await _context.Facilities
                .Where(f => f.Status == FacilityStatus.Pending)
                .OrderBy(f=> f.CreatedAt)
                .Select(f => new PendingFacilityResponse(
                    f.Id,
                    f.Name,
                    f.City,
                    f.District,
                    f.CreatedAt
                    )).ToListAsync();

            return Ok(pendingFacilities);
        }
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> ApproveFacility(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);

            if(facility == null)
            {
                return NotFound();
            }

            try
            {
                facility.Approve();

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(InvalidOperationException ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}
