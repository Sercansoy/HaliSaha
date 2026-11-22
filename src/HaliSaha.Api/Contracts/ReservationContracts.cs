using HaliSaha.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace HaliSaha.Api.Contracts
{
    public class CreateReservationRequest
    {
        [Required]
        public int PitchId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }
    }

    public class ReservationResponse
    {
        public int Id { get; set; }
        public int PitchId { get; set; }
        public DateTime StartTime { get; set; }
        public ReservationStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class CancelReservationRequest
    {
        public string? Reason {  get; set; }
    }
}
