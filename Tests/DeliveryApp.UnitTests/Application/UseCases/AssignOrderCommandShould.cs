using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrder;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Services.DispatchService;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases;

public class AssignOrderCommandShould
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly ICourierRepository _courierRepositoryMock;
    private readonly IDispatchService _dispatchServiceMock;
    
    public AssignOrderCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _courierRepositoryMock = Substitute.For<ICourierRepository>();
        _dispatchServiceMock = Substitute.For<IDispatchService>();
    }

    [Fact]
    public async Task ReturnTrueWhenOrderIsAssigned()
    {
        //Arrange
        _orderRepositoryMock.GetFirstCreatedAsync(CancellationToken.None).Returns(Task.FromResult(CreatedOrder()));
        _courierRepositoryMock.GetAllFreeAsync(CancellationToken.None).Returns(Task.FromResult(FreeCouriers()));
        _dispatchServiceMock.Dispatch(Arg.Any<Order>(), Arg.Any<List<Courier>>()).Returns(FreeCouriers().Last());
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

        var command = new AssignOrderCommand();
        var handler = new AssignOrderHandler(
            _orderRepositoryMock,
            _courierRepositoryMock,
            _dispatchServiceMock,
            _unitOfWork);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ThrowExceptionWhenFreeCourierIsNotFound()
    {
        //Arrange
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));
        _orderRepositoryMock.GetFirstCreatedAsync(CancellationToken.None).Returns(Task.FromResult(CreatedOrder()));
        _courierRepositoryMock.GetAllFreeAsync(CancellationToken.None).Returns(Task.FromResult(AssignedCouriers()));
        _dispatchServiceMock.Dispatch(Arg.Any<Order>(), Arg.Any<List<Courier>>()).Returns(FreeCouriers().Last());

        var command = new AssignOrderCommand();
        var handler = new AssignOrderHandler(
            _orderRepositoryMock,
            _courierRepositoryMock,
            _dispatchServiceMock,
            _unitOfWork);
        
        //Act
        var result = () => handler.Handle(command, CancellationToken.None).Result;
        
        //Assert
        result.Should().Throw<Exception>();
    }

    private Order CreatedOrder() => Order.Create(Guid.NewGuid(), new Location(1, 1));

    private ICollection<Courier> FreeCouriers() =>
    [
        Courier.Create("Олег", "Пешком", 1, new Location(4, 4)),
        Courier.Create("Иван", "Велосипед", 2, new Location(1, 1)),
        Courier.Create("Сергей", "Самокат", 3, new Location(1, 1))
    ];

    private ICollection<Courier> AssignedCouriers()
    {
        return Array.Empty<Courier>();
    }
}