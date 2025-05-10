using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Core.Ports;

public interface IQueryDbContext
{
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<Courier> Couriers { get; set; }
}