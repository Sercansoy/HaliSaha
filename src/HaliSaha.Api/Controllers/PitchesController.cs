using HaliSaha.Api.Contracts;
using HaliSaha.Core.Entities;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HaliSaha.Api.Controllers
{
    [ApiController]
    [Route("api/pitches")]
    public class PitchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PitchesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}/availability")]
        [AllowAnonymous]
        public async Task<ActionResult<PitchAvailabilityResponse>> GetAvailability(
            int id,
            [FromQuery] DateOnly? date = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var targetDay = date ?? today;

            if(targetDay < today || targetDay > today.AddDays(2))
            {
                return BadRequest("Sadece bugün ve sonraki 2 gün sorgulanabilir.");
            }

            var pitchExists = await _context.Facilities
                .Where(f=>f.Status == FacilityStatus.Approved)
                .SelectMany(f=> f.Pitches)
                .AnyAsync(p=> p.Id == id && p.IsActive);

            if (!pitchExists)
            {
                return NotFound();
            }

            var dayStart = targetDay.ToDateTime(TimeOnly.MinValue);
            var dayEnd = dayStart.AddDays(1);

            var bookedStartTimes = await _context.Reservations
                .Where(r => r.PitchId == id
                && r.StartTime >= dayStart
                && r.StartTime < dayEnd
                && r.Status != ReservationStatus.Cancelled)
                .Select(r => r.StartTime)
                .ToListAsync();

            var bookedHours = bookedStartTimes.Select(t => t.Hour).ToHashSet();

            var now = DateTime.Now;
            var slots = new List<AvailabilitySlotResponse>();

            for(var hour = ReservationRules.FirstStartHour; hour<= ReservationRules.LastStartHour; hour++)
            {
                var slotStart = targetDay.ToDateTime(new TimeOnly(hour, 0));

                SlotStatus status;

                if (slotStart <= now)
                    status = SlotStatus.Past;
                else if (bookedHours.Contains(hour))
                    status = SlotStatus.Booked;
                else
                    status = SlotStatus.Available;

                slots.Add(new AvailabilitySlotResponse(new TimeOnly(hour, 0), status));
            }

            return Ok(new PitchAvailabilityResponse(id, targetDay, slots));
        }
    }
}
