using HaliSaha.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HaliSaha.Api.Contracts
{
    public record FacilityResponse(
        int Id,
        string Name,
        string City,
        string District,
        string Address,
        string Phone,
        IEnumerable<PitchResponse> Pitches)
    {
        public static Expression<Func<Facility, FacilityResponse>> Projection => f => new FacilityResponse(
            f.Id,
            f.Name,
            f.City,
            f.District,
            f.Address,
            f.Phone,
            f.Pitches
            .Where(p => p.IsActive)
            .Select(p => new PitchResponse(p.Id, p.Name, p.IsIndoor))
            .ToList()
            );
    }
    public record PendingFacilityResponse(
        int Id,
        string Name,
        string City,
        string District,
        DateTime CreatedAt);

    public record CreateFacilityRequest(
        [Required, MaxLength(100)] string Name,
        [Required, MaxLength(50)] string City,
        [Required, MaxLength(50)] string District,
        [Required, MaxLength(200)] string Address,
        [Required, MaxLength(20)] string Phone);

    public record CreatePitchRequest(
        [Required, MaxLength(100)] string Name,
        bool IsIndoor);
}
