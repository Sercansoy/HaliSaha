using HaliSaha.Core.Exceptions;

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
        public List<Pitch> Pitches { get; set; } = new();
        public Pitch AddPitch(string name,bool isIndoor)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Saha adı boş veya yalnızca boşluk karakterlerinden oluşuyor olamaz");
            }

            string trimmedName = name.Trim();

            bool isAlreadyExists = Pitches.Any(p => string.Equals(p.Name, trimmedName, StringComparison.OrdinalIgnoreCase));
            if (isAlreadyExists)
            {
                throw new DomainException($"'{trimmedName}' adında bir saha bu tesiste zaten mevcut.");
            }

            var newPitch = new Pitch
            {
                Name = trimmedName,
                IsIndoor = isIndoor,
            };
            Pitches.Add(newPitch);

            return newPitch;
                
        }
    }
}
