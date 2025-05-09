using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryShould: IAsyncLifetime
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
    public async Task CreateCourier()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Act
        var repository = new CourierRepository(_context);
        await repository.CreateAsync(courier, CancellationToken.None);
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbCourier = await repository.GetByIdAsync(courier.Id, CancellationToken.None);
        courier.Should().BeEquivalentTo(dbCourier);
    }
    
    [Fact]
    public async Task UpdateCourier()
    {
        //Arrange
        var courier = Courier.Create("Иван", "Велосипед", 2, new Location(1, 2));
        
        //Act
        var repository = new CourierRepository(_context);
        await repository.CreateAsync(courier, CancellationToken.None);
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();
        
        courier.SetBusy();
        repository.Update(courier);
        
        await unitOfWork.SaveChangesAsync();

        //Assert
        var dbCourier = await repository.GetByIdAsync(courier.Id, CancellationToken.None);
        dbCourier.Status.Name.Should().Be(courier.Status.Name);
    }

    [Fact]
    public async Task GetAllFree()
    {
        //Arrange
        var couriers = new[]
        {
            Courier.Create("Олег", "Пешком", 1, new Location(4, 4)),
            Courier.Create("Иван", "Велосипед", 2, new Location(1, 1)),
            Courier.Create("Сергей", "Самокат", 3, new Location(1, 1)),
        };
        
        couriers.First().SetBusy();
        
        //Act
        var repository = new CourierRepository(_context);
        foreach (var courier in couriers)
        {
            await repository.CreateAsync(courier, CancellationToken.None);
        }
        
        var unitOfWork = new UnitOfWork(_context);
        await unitOfWork.SaveChangesAsync();
        
        //Assert
        var dbCouriers = await repository.GetAllFreeAsync(CancellationToken.None);
        dbCouriers.Should().NotBeNull();
        dbCouriers.Count.Should().Be(2);
    }
}