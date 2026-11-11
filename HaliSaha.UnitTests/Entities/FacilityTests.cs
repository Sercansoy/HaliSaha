using HaliSaha.Core.Entities;
using HaliSaha.Core.Exceptions;

namespace HaliSaha.UnitTests.Entities
{
    public class FacilityTests
    {
        [Fact]
        public void AddPitch_ValidName_AddsPitchToFacility()
        {
            // Arrange (Hazırlama)
            var facility = new Facility { Name = "Test Tesisi" };
            // Act(Çaliştirma)
            var pitch = facility.AddPitch("Saha 1", isIndoor: true);

            // Assert (Doğrulama)
            Assert.Single(facility.Pitches);
            Assert.Equal("Saha 1", pitch.Name);
            Assert.True(pitch.IsIndoor);
            Assert.True(pitch.IsActive);
        }

        [Fact]
        public void AddPitch_EmptyName_ThrowsDomainException()
        {
            //Arrange
            var facility = new Facility();
            string emptyName = "";
            //Act & Assert
            Assert.Throws<DomainException>(() => facility.AddPitch(emptyName, true));
        }

        [Fact]
        public void AddPitch_NameWithSpaces_TrimsName()
        {
            // Arrange (Hazırlama)
            var facility = new Facility();
            string nameWithSpaces = " Saha 1 ";
            // Act (Çalıştırma)
            var pitch = facility.AddPitch(nameWithSpaces, true);
            // Assert (Doğrulama)
            Assert.Equal("Saha 1", pitch.Name);
        }

        [Fact]
        public void AddPitch_DuplicateNameDifferentCase_ThrowsDomainException()
        {
            // Arrange (Hazırlama)
            var facility = new Facility();

            facility.AddPitch("Saha 1", true);

            // Act & Assert (Çalıştırma ve Doğrulama)
            Assert.Throws<DomainException>(() => facility.AddPitch("saha 1", true));
        }

        [Fact]
        public void AddPitch_DuplicateNameWithSpaces_ThrowsDomainException()
        {
            var facility = new Facility();
            facility.AddPitch("Saha 1", true);

            Assert.Throws<DomainException>(() => facility.AddPitch(" Saha 1", true));
        }
    }
}
