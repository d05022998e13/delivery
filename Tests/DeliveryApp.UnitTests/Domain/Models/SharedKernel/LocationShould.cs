using System;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel;

public class LocationShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        
        //Act
        var location = new Location(3, 7);
        
        //Assert
        location.Should().NotBeNull();
        location.X.Should().Be(3);
        location.Y.Should().Be(7);
    }

    [Theory]
    [InlineData(0, 7)]
    [InlineData(7, 0)]
    [InlineData(-2, -2)]
    public void ThrowExceptionWhenParamsIsNotCorrect(int x, int y)
    {
        //Arrange
        
        //Act
        var result = () => new Location(x, y);
        
        //Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Fact]
    public void BeEqualWhenParamsAreEqual()
    {
        //Arrange
        var pointA = new Location(3, 7);
        var pointB = new Location(3, 7);
        
        //Act
        var result1 = pointA == pointB;
        var result2 = pointA.Equals(pointB);
        var result3 = pointB.Equals(pointA);
        
        //Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
    }
    
    [Fact]
    public void BeCorrectOnRandom()
    {
        //Arrange
        
        //Act
        var location = Location.Random();
        
        //Assert
        location.Should().NotBeNull();
        location.X.Should().BeGreaterThanOrEqualTo(Location.Min);
        location.X.Should().BeLessThanOrEqualTo(Location.Max);
        location.Y.Should().BeGreaterThanOrEqualTo(Location.Min);
        location.Y.Should().BeLessThanOrEqualTo(Location.Max);
    }
    
    [Fact]
    public void BeCorrectOnCalculateDistance()
    {
        //Arrange
        var pointA = new Location(1, 2);
        var pointB = new Location(2, 1);
        
        //Act
        var distance1 = pointA.DistanceTo(pointB);
        var distance2 = pointB.DistanceTo(pointA);
        
        //Assert
        distance1.Should().Be(2);
        distance2.Should().Be(2);
    }
    
    [Fact]
    public void BeDifferentOnRandom()
    {
        //Arrange
        var locationA = Location.Random();
        var locationB = Location.Random();
        
        //Act
        var result = locationA.X == locationB.X && locationA.Y == locationB.Y;
        
        //Assert
        result.Should().BeFalse();
    }
}