using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class TransportShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        
        //Act
        var transport = new Transport("Велосипед", 2);
        
        //Assert
        transport.Should().NotBeNull();
        transport.Name.Should().Be("Велосипед");
        transport.Speed.Value.Should().Be(2);
    }
    
    [Theory]
    [InlineData("", 7)]
    [InlineData("Name1", 0)]
    [InlineData("", -2)]
    public void ThrowExceptionWhenParamsIsNotCorrect(string name, int speedValue)
    {
        //Arrange
        
        //Act
        var result = () => new Transport(name, speedValue);
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void BeNotEqualWhenIdNotEquals()
    {
        //Arrange
        var transport1 = new Transport("Самокат", 1);
        var transport2 = new Transport("Самокат", 1);
        
        //Act
        var result = transport1 == transport2;
        
        //Assert
        transport1.Id.Should().NotBe(transport2.Id);
        result.Should().BeFalse();
    }
    
    [Fact]
    public void BeCorrectOnMoveUp()
    {
        //Arrange
        var pointA = new Location(1, 1);
        var pointB = new Location(1, 9);
        var transport = new Transport("Самокат", 2);
        
        //Act
        var result = transport.Move(pointA, pointB);
        
        //Assert
        result.X.Should().Be(1);
        result.Y.Should().Be(3);
    }
    
    [Fact]
    public void BeCorrectOnMoveDown()
    {
        //Arrange
        var pointA = new Location(4, 2);
        var pointB = new Location(1, 1);
        var transport = new Transport("Самокат", 2);
        
        //Act
        var result = transport.Move(pointA, pointB);
        
        //Assert
        result.X.Should().Be(3);
        result.Y.Should().Be(1);
    }
}