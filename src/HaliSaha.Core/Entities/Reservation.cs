
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
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CancellationReason { get; set; }
    }
}
