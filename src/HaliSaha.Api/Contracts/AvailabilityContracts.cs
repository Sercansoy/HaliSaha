namespace HaliSaha.Api.Contracts
{
    public enum SlotStatus
    {
        Available = 0,
        Booked = 1,
        Past = 2
    }

    public sealed record AvailabilitySlotResponse(
        TimeOnly StartTime,
        SlotStatus Status
        );

    public sealed record PitchAvailabilityResponse(
        int PitchId,
        DateOnly Date,
        IReadOnlyList<AvailabilitySlotResponse> Slots
        );
}
