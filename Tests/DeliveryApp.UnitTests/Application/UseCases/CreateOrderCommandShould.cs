using System;
using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using Xunit;

namespace DeliveryApp.UnitTests.Application.UseCases;

public class CreateOrderCommandShould
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepositoryMock;
    
    public CreateOrderCommandShould()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
    }
    
    [Fact]
    public async Task ReturnTrueWhenOrderIsCreated()
    {
        //Arrange
        var id = Guid.NewGuid();
        
        _orderRepositoryMock.GetByIdAsync(id, CancellationToken.None).Returns(Task.FromResult<Order>(null));
        _orderRepositoryMock.CreateAsync(Arg.Any<Order>(), CancellationToken.None).Returns(Task.CompletedTask);
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

        var command = new CreateOrderCommand(id, nameof(CreateOrderCommand.Street));
        var handler = new CreateOrderHandler(_orderRepositoryMock, _unitOfWork);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void ThrowExceptionWhenOrderAlreadyExists()
    {
        //Arrange
        var id = Guid.NewGuid();
        
        _orderRepositoryMock.GetByIdAsync(id, CancellationToken.None).Returns(Task.FromResult(Order.Create(id, new Location(1, 1))));
        _orderRepositoryMock.CreateAsync(Arg.Any<Order>(), CancellationToken.None).Returns(Task.CompletedTask);
        _unitOfWork.SaveChangesAsync().Returns(Task.FromResult(true));

        var command = new CreateOrderCommand(id, nameof(CreateOrderCommand.Street));
        var handler = new CreateOrderHandler(_orderRepositoryMock, _unitOfWork);

        //Act
        var result = () => handler.Handle(command, CancellationToken.None).Result;
        
        //Assert
        result.Should().Throw<Exception>();
    }
}