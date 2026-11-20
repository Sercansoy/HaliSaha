using HaliSaha.Api.Contracts;
using HaliSaha.Core.Entities;
using HaliSaha.Core.Exceptions;
using HaliSaha.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;

namespace HaliSaha.Api.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const int FirstStartHour = 12;
        private const int LastStartHour = 22;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = nameof(UserRole.Player))]
        public async Task<ActionResult<ReservationResponse>> GetReservation(int id)
        {
            // 1. Token'dan giriş yapan kullanıcının Id'sini al
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if(!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // 2. Rezervasyonu id ile veritabanında bul
            var reservation = await _context.Reservations.FirstOrDefaultAsync(x=>x.Id == id);

            // 3. Yoksa VEYA başkasına aitse 404 dön
            if(reservation == null || reservation.UserId != userId)
            {
                return NotFound();
            }

            // 4. ReservationResponse'a çevirip dön
            var response = new ReservationResponse
            {
                Id = reservation.Id,
                PitchId = reservation.PitchId,
                StartTime = reservation.StartTime,
                Status = reservation.Status,
                CancellationReason = reservation.CancellationReason
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Player))]
        public async Task<ActionResult<ReservationResponse>> CreateReservation(CreateReservationRequest request)
        {
            // 1. Token'dan kullanıcının Id'sini al
            var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if(!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            // 2. Saha var mı? Yoksa 404
            var pitch = await _context.Pitches.FirstOrDefaultAsync(x => x.Id == request.PitchId && x.IsActive);

            if(pitch == null)
            {
                return NotFound();
            }

            // 3. Saat kontrolleri, hepsi 400:
            //    - geçmişte mi?
            var now = DateTime.Now;
            if(request.StartTime <= now)
            {
                return BadRequest("Geçmişte bir rezervasyon yapılamaz");
            }
            //    - 48 saatten ileride mi?
            if(request.StartTime > now.AddHours(48))
            {
                return BadRequest("En fazla 48 saat sonrası için rezervasyon yapılabilir");
            }
            //    - tam saat mi? (dakika ve saniye 0 olmalı)
            if(request.StartTime.Minute != 0 || request.StartTime.Second  != 0)
            {
                return BadRequest("Başlangıç saati tam saat olmalıdır");
            }
            if (request.StartTime.Hour < FirstStartHour || request.StartTime.Hour > LastStartHour)
            {
                return BadRequest($"Rezervasyon başlangıç saati {FirstStartHour} ile {LastStartHour} saati arasında olmalıdır");
            }

            // 4. Yeni Reservation nesnesi oluştur
            //    (UserId token'dan, durum Bekliyor, oluşturulma zamanı şu an)
            var reservation = new Reservation
            {
                PitchId = request.PitchId,
                StartTime = request.StartTime,
                UserId = userId,
                Status = ReservationStatus.Pending,
                CreatedAt = now
            };
            // 5. Kaydet
            _context.Reservations.Add(reservation);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 })
            {
                return Conflict("Bu saha ve saat için zaten bir rezervasyon mevcut.");
            }

            // 6. 201 dön: CreatedAtAction ile GetReservation'a işaret et
            var response = new ReservationResponse
            {
                Id = reservation.Id,
                PitchId = reservation.PitchId,
                StartTime = reservation.StartTime,
                Status = reservation.Status,
                CancellationReason = reservation.CancellationReason
            };

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, response);
        }

    }
}
