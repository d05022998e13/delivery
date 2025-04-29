using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.CourierAggregate;

public class CourierShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        
        //Act
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Assert
        courier.Should().NotBeNull();
        courier.Name.Should().Be("Иван");
        courier.Transport.Name.Should().Be("Велосипед");
        courier.Transport.Speed.Value.Should().Be(2);
        courier.Location.X.Should().Be(1);
        courier.Location.Y.Should().Be(2);
        courier.Status.Should().Be(CourierStatus.Free);
    }
    
    [Theory]
    [InlineData("", "Велосипед", 2)]
    [InlineData("Иван", "", 0)]
    [InlineData("Иван", "Велосипед", 1000)]
    public void ThrowExceptionWhenParamsIsNotCorrect(string name, string transportName, int transportSpeed)
    {
        //Arrange
        var location = new Location(1, 2);
        
        //Act
        var result = () => Courier.Create(name, transportName, transportSpeed, location);
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void BeCorrectOnBusyWhenFree()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        //Act
        courier.SetBusy();
        
        //Assert
        courier.Status.Should().Be(CourierStatus.Busy);
    }
    
    [Fact]
    public void BeCorrectOnFreeWhenBusy()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Act
        courier.SetBusy();
        courier.SetFree();
        
        //Assert
        courier.Status.Should().Be(CourierStatus.Free);
    }
    
    [Fact]
    public void ThrowExceptionOnFreeWhenFree()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Act
        var result = () => courier.SetFree();
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void ThrowExceptionOnBusyWhenBusy()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Act
        courier.SetBusy();
        var result = () => courier.SetBusy();
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void BeCorrectOnCalculateTimeToLocation()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 1));
        
        //Act
        var result = courier.CalculateTimeToLocation(new Location(5, 5));
        
        //Assert
        result.Should().Be(4);
    }
    
    [Fact]
    public void BeCorrectOnMove()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(4, 2));
        
        //Act
        courier.Move(new Location(1, 1));
        var result = courier.Location;
        
        //Assert
        result.X.Should().Be(3);
        result.Y.Should().Be(1);
    }
}