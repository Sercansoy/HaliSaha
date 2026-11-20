
using HaliSaha.Core.Exceptions;

namespace HaliSaha.Core.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        public Pitch Pitch { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CancellationReason { get; set; }

        public void CancelByPlayer(DateTime now)
        {
            if (Status == ReservationStatus.Cancelled)
                throw new DomainException("Rezervasyon zaten iptal edilmiş durumda.");
            if (StartTime <= now)
                throw new DomainException("Geçmiş rezervasyonlar iptal edilemez.");
            if (StartTime < now.AddHours(24))
                throw new DomainException("Rezervasyonlar yalnızca başlangıç ​​saatinden en az 24 saat önce iptal edilebilir.");
            Status = ReservationStatus.Cancelled;
        }
        public void CancelByOwner(DateTime now, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("İptal nedeni gereklidir.");
            if (Status == ReservationStatus.Cancelled)
                throw new DomainException("Rezervasyon zaten iptal edilmiş");
            if (StartTime <= now)
                throw new DomainException("Geçmiş rezervasyonlar iptal edilemez");
            Status = ReservationStatus.Cancelled;
            CancellationReason = reason.Trim();
        }

    }
}
