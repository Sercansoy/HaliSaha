using HaliSaha.Core.Entities;
using HaliSaha.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HaliSaha.UnitTests.Entities
{
    public class ReservationTests
    {
        private static readonly DateTime now = new DateTime(2026, 11, 20, 12, 0, 0);

        [Fact]
        public void CancelByPlayer_MoreThan24HoursBefore_CancelsReservation()
        {
            //Arrange
            var reservation = new Reservation { StartTime = now.AddHours(30) };

            //Act
            reservation.CancelByPlayer(now);

            //Assert
            Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        }

        [Fact]
        public void CancelByPlayer_LessThan24HoursBefore_ThrowsDomainException()
        {
            var reservation = new Reservation { StartTime = now.AddHours(23) };

            Assert.Throws<DomainException>(() => reservation.CancelByPlayer(now));
        }

        [Fact]
        public void CancelByPlayer_AlreadyCancelled_ThrowsDomainException()
        {
            var reservation = new Reservation { StartTime = now.AddHours(30)};

            reservation.CancelByPlayer(now);

            Assert.Throws<DomainException>( () => reservation.CancelByPlayer(now));
        }

        [Fact]
        public void CancelByOwner_WithReason_CancelsAndSavesReason()
        {
            var reservation = new Reservation { StartTime = now.AddHours(2)};

            reservation.CancelByOwner(now, " Su borusu patladı ");

            Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
            Assert.Equal("Su borusu patladı", reservation.CancellationReason);
        }

        [Fact]
        public void CancelByOwner_EmptyReason_ThrowsDomainException()
        {
            var reservation = new Reservation { StartTime = now.AddHours(2) };

            Assert.Throws<DomainException>(() => reservation.CancelByOwner(now, " "));
        }
    }
}
