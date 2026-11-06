namespace HaliSaha.Core.Entities
{
    public class Pitch
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsIndoor { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
