using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases;

public class MoveCouriersCommandShould
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly ICourierRepository _courierRepositoryMock;
    
    public MoveCouriersCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _courierRepositoryMock = Substitute.For<ICourierRepository>();
    }
    
    [Fact]
    public async Task ReturnTrueWhenCouriersAreMoved()
    {
        //Arrange
        _orderRepositoryMock.GetAllAssignedAsync(CancellationToken.None).Returns(Task.FromResult(AssignedOrders()));
        _courierRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None).Returns(Task.FromResult(AssignedCourier()));
        _orderRepositoryMock.Update(AssignedOrders().First());
        _courierRepositoryMock.Update(AssignedCourier());
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

        var command = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_courierRepositoryMock, _orderRepositoryMock, _unitOfWork);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ThrowExceptionWhenAssignedOrdersNotFound()
    {
        //Arrange
        _orderRepositoryMock.GetAllAssignedAsync(CancellationToken.None).Returns(Task.FromResult<ICollection<Order>>(Array.Empty<Order>()));
        _courierRepositoryMock.GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None).Returns(Task.FromResult(AssignedCourier()));
        _orderRepositoryMock.Update(AssignedOrders().First());
        _courierRepositoryMock.Update(AssignedCourier());
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

        var command = new MoveCouriersCommand();
        var handler = new MoveCouriersHandler(_courierRepositoryMock, _orderRepositoryMock, _unitOfWork);

        //Act
        var result = () => handler.Handle(command, CancellationToken.None).Result;
        
        //Assert
        result.Should().Throw<Exception>();
    }

    private ICollection<Order> AssignedOrders()
    {
        var orders = new List<Order>
        {
            Order.Create(Guid.NewGuid(), new Location(1, 1))
        };
        
        orders.ForEach(order => order.Assign(AssignedCourier().Id));
        
        return orders;
    }

    private Courier AssignedCourier()
    {
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        courier.SetBusy();
        
        return courier;
    }
}