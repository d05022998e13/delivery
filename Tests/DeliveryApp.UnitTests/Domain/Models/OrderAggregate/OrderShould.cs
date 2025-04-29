using System;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Models.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void BeCorrectWhenParamsIsCorrect()
    {
        //Arrange
        var guid = Guid.NewGuid();
        var location = new Location(1, 2);
        
        //Act
        var order = Order.Create(guid, location);
        
        //Assert
        order.Should().NotBeNull();
        order.Id.Should().Be(guid);
        order.Location.X.Should().Be(1);
        order.Location.Y.Should().Be(2);
        order.Status.Should().Be(OrderStatus.Created);
    }
    
    [Fact]
    public void ThrowExceptionWhenGuidIsNotCorrect()
    {
        //Arrange
        var guid = Guid.Empty;
        var location = new Location(1, 2);
        
        //Act
        var result = () => Order.Create(guid, location);
        
        //Assert
        result.Should().Throw<Exception>();
    }

    [Fact]
    public void BeCorrectOnAssign()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(1, 2));
        
        //Act
        order.Assign(Guid.NewGuid());
        
        //Assert
        order.Status.Should().Be(OrderStatus.Assigned);
    }
    
    [Fact]
    public void BeCorrectOnComplete()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(1, 2));
        order.Assign(Guid.NewGuid());
        
        //Act
        order.Complete();
        
        //Assert
        order.Status.Should().Be(OrderStatus.Completed);
    }
    
    [Fact]
    public void ThrowExceptionWhenAsignCompleted()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(1, 2));
        order.Assign(Guid.NewGuid());
        order.Complete();
        
        //Act
        var result = () => order.Assign(Guid.NewGuid());
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void ThrowExceptionWhenCompleteCreated()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(1, 2));
        
        //Act
        var result = () => order.Complete();
        
        //Assert
        result.Should().Throw<Exception>();
    }
}