namespace HaliSaha.Api.Contracts
{
    public record FacilityResponse(
        int Id,
        string Name,
        string City,
        string District,
        string Address,
        string Phone,
        IEnumerable<PitchResponse> Pitches
        );
}
