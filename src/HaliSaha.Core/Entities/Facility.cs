namespace HaliSaha.Core.Entities
{
    public class Facility
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address {  get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public FacilityStatus Status { get; set; }

        public List<Pitch> Pitchs { get; set; } = new();
    }
}
