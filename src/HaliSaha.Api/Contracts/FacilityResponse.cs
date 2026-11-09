using HaliSaha.Core.Entities;
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
}
