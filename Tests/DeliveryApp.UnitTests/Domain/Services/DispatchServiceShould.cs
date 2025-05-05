using System;
using System.Collections.Generic;
using System.Linq;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Services.DispatchService;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceShould
{
    private readonly DispatchService _dispatchService = new();
    
    [Fact]
    public void BeCorrectOnDispatch()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(8, 4));
        var couriers = new List<Courier>
        {
            Courier.Create("Олег", "Пешком", 1, new Location(4, 4)),
            Courier.Create("Иван", "Велосипед", 2, new Location(1, 1)),
            Courier.Create("Сергей", "Самокат", 3, new Location(1, 1)),
        };
        
        var courierId = couriers.First(x => x.Name == "Сергей").Id;
        
        //Act
        _dispatchService.Dispatch(order, couriers);
        
        //Assert
        order.Status.Should().Be(OrderStatus.Assigned);
        order.CourierId.Should().Be(courierId);
    }
    
    [Fact]
    public void ThrowExceptionOnDispatchWhenAllCouriersAreBusy()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(8, 4));
        var couriers = new List<Courier>
        {
            Courier.Create("Олег", "Пешком", 1, new Location(4, 4)),
            Courier.Create("Иван", "Велосипед", 2, new Location(1, 1)),
            Courier.Create("Сергей", "Самокат", 3, new Location(1, 1)),
        };
        couriers.ForEach(x => x.SetBusy());
        
        //Act
        var result = () => _dispatchService.Dispatch(order, couriers);
        
        //Assert
        result.Should().Throw<Exception>();
    }
    
    [Fact]
    public void ThrowExceptionOnDispatchWhenOrderNotCreated()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(8, 4));
        var couriers = new List<Courier>
        {
            Courier.Create("Олег", "Пешком", 1, new Location(4, 4)),
            Courier.Create("Иван", "Велосипед", 2, new Location(1, 1)),
            Courier.Create("Сергей", "Самокат", 3, new Location(1, 1)),
        };
        order.Assign(Guid.NewGuid());
        
        //Act
        var result = () => _dispatchService.Dispatch(order, couriers);
        
        //Assert
        result.Should().Throw<Exception>();
    }
}