using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    private ApplicationDbContext _context;
    
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("basket")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();
    
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
            _postgreSqlContainer.GetConnectionString(),
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
            }).Options;
        
        _context = new ApplicationDbContext(contextOptions);
        await _context.Database.MigrateAsync();

    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task CreateOrder()
    {
        //Arrange
        var order = Order.Create(Guid.NewGuid(), new Location(1, 3));
        
        //Act
        var repository = new OrderRepository(_context);
        await repository.CreateAsync(order, CancellationToken.None);
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbOrder = await repository.GetByIdAsync(order.Id, CancellationToken.None);
        order.Should().BeEquivalentTo(dbOrder);
    }

    [Fact]
    public async Task UpdateOrder()
    {
        //Arrange
        var courierId = Guid.NewGuid();
        
        var order = Order.Create(Guid.NewGuid(), new Location(1, 3));
        
        //Act
        var repository = new OrderRepository(_context);
        await repository.CreateAsync(order, CancellationToken.None);
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();
        
        order.Assign(courierId);
        repository.Update(order);
        
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbOrder = await repository.GetByIdAsync(order.Id, CancellationToken.None);
        dbOrder.Status.Name.Should().Be(order.Status.Name);
    }

    [Fact]
    public async Task GetCreated()
    {
        //Arrange
        var orders = new[]
        {
            Order.Create(Guid.NewGuid(), new Location(1, 1)),
            Order.Create(Guid.NewGuid(), new Location(1, 3))
        };
        
        //Act
        var repository = new OrderRepository(_context);
        foreach (var order in orders)
        {
            await repository.CreateAsync(order, CancellationToken.None);
        }
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbOrder = await repository.GetFirstCreatedAsync(CancellationToken.None);
        dbOrder.Should().NotBeNull();
        dbOrder.Status.Name.Should().Be(OrderStatus.Created.Name);
    }

    [Fact]
    public async Task GetAllAssigned()
    {
        //Arrange
        var orders = new[]
        {
            Order.Create(Guid.NewGuid(), new Location(1, 1)),
            Order.Create(Guid.NewGuid(), new Location(1, 3))
        };
        
        var assigned = orders.First();
        assigned.Assign(Guid.NewGuid());
        
        //Act
        var repository = new OrderRepository(_context);
        foreach (var order in orders)
        {
            await repository.CreateAsync(order, CancellationToken.None);
        }
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbOrders = await repository.GetAllAssignedAsync(CancellationToken.None);
        dbOrders.Should().NotBeNull();
        dbOrders.Count.Should().Be(1);
        dbOrders.First().Should().Be(assigned);
    }
}