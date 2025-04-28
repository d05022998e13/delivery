using System;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.SharedKernel;

public class SpeedShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        
        //Act
        var speed = new Speed(3);
        
        //Assert
        speed.Should().NotBeNull();
        speed.Value.Should().Be(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-2)]
    public void ThrowExceptionWhenParamsIsNotCorrect(int value)
    {
        //Arrange
        
        //Act
        var result = () => new Speed(value);
        
        //Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }
    
    [Fact]
    public void BeEqualWhenParamsAreEqual()
    {
        //Arrange
        var speed1 = new Speed(3);
        var speed2 = new Speed(3);
        
        //Act
        var result1 = speed1 == speed2;
        var result2 = speed1.Equals(speed2);
        var result3 = speed2.Equals(speed1);
        
        //Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
    }
}