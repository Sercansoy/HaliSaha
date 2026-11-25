using HaliSaha.Api.Contracts;
using HaliSaha.Core.Entities;
using HaliSaha.Core.Exceptions;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;

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

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.FacilityOwner))]
        public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityRequest request)
        {
            // 1. Giriş yapan kullanıcının Id'sini token'dan al
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // 2. Tesisi oluştur (Status otomatik olarak Pending başlar)
            var facility = new Facility
            {
                Name = request.Name.Trim(),
                City = request.City.Trim(),
                District = request.District.Trim(),
                Address = request.Address.Trim(),
                Phone = request.Phone.Trim(),
                OwnerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Kaydet
            _context.Facilities.Add(facility);
            await _context.SaveChangesAsync();

            // 4. 201 Created dön
            var response = new FacilityResponse(
                facility.Id,
                facility.Name,
                facility.City,
                facility.District,
                facility.Address,
                facility.Phone,
                []);

            return CreatedAtAction(nameof(GetFacility), new { id = facility.Id }, response);
        }

        [HttpPost("{facilityId:int}/pitches")]
        [Authorize(Roles = nameof(UserRole.FacilityOwner))]
        public async Task<IActionResult> AddPitch(int facilityId, [FromBody] CreatePitchRequest request)
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if(!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var facility = await _context.Facilities
                .Include(f=> f.Pitches)
                .FirstOrDefaultAsync(f => f.Id == facilityId);

            if(facility == null)
            {
                return NotFound();
            }
            //IDOR (Insecure Direct Object Reference) kısmı
            if (facility.OwnerId != userId)
            {
                return Forbid();
            }

            try
            {
                var pitch = facility.AddPitch(request.Name, request.IsIndoor);
                await _context.SaveChangesAsync();

                var response = new PitchResponse
                (
                    pitch.Id,
                    pitch.Name,
                    pitch.IsIndoor
                );

                return CreatedAtAction(nameof(GetFacility), new { id = facilityId }, response);
            }
            catch(DomainException ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status409Conflict);
            }

        }

        [HttpGet("{facilityId:int}/reservations")]
        [Authorize(Roles = nameof(UserRole.FacilityOwner))]
        public async Task<ActionResult<List<DailyPitchResponse>>> GetDailyReservations(
            int facilityId,
            [FromQuery] DateOnly? date = null)
        {
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if(!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var targetDay = date ?? DateOnly.FromDateTime(DateTime.Now);

            var dayStart = targetDay.ToDateTime(TimeOnly.MinValue);
            var dayEnd = dayStart.AddDays(1);

            var isOwner = await _context.Facilities
                .AnyAsync(f => f.Id == facilityId && f.OwnerId == userId);
            if (!isOwner)
            {
                return NotFound();
            }

            var result = await _context.Pitches
                .Where(p => p.FacilityId == facilityId)
                .OrderBy(p => p.Id)
                .Select(p => new DailyPitchResponse(
                    p.Id,
                    p.Name,
                    _context.Reservations
                    .Where(r => r.PitchId == p.Id
                    && r.StartTime >= dayStart
                    && r.StartTime < dayEnd)
                    .OrderBy(r => r.StartTime)
                    .Select(r => new DailyReservationResponse(
                        r.Id,
                        r.StartTime,
                        r.Status,
                        r.User.FullName))
                    .ToList()))
                .ToListAsync();

            return Ok(result);
        }
        
    }
}
