using System;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class GetAgeTest
    {
        [Test]
        public void ShouldReturnAgeWhenSubtractingTodayDateAndGivenDate()
        {
            // arrange
            var now = new DateTime(2013, 7, 26);
            var bday = new DateTime(1991, 7, 27);

            // act
            var age = now.Year - bday.Year;
            if (bday > now.AddYears(-age)) age--;

            // assert
            age.Should().Be(21);
        }
    }
}
