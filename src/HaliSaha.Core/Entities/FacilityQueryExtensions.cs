namespace HaliSaha.Core.Entities
{
    public static class FacilityQueryExtensions
    {
        // Oyuncuların görebileceği ve kiralayabileceği sahalar:
        // tesis onaylı VE saha aktif. IQueryable döndüğü için filtre SQL'de çalışır.
        public static IQueryable<Pitch> BookablePitches(this IQueryable<Facility> facilities)
        {
            return facilities
                .Where(f => f.Status == FacilityStatus.Approved)
                .SelectMany(f => f.Pitches)
                .Where(p => p.IsActive);
        }
    }
}
