using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public sealed class Transport : Entity<Guid>
{
    public string Name { get; private set; }
    
    public Speed Speed { get; private set; }
    
    private Transport() {}

    public Transport(string name, int speed)
    {
        Id = Guid.NewGuid();
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Не указано наименование транспорта")
            : name;
        Speed = new Speed(speed);
    }

    public Location Move(Location from, Location to)
    {
        if (from.DistanceTo(to) <= Speed.Value)
        {
            return to;
        }

        var ability = Speed.Value;

        var x = from.X;
        var y = from.Y;
        var isBackX = from.X > to.X;
        var isBackY = from.Y > to.Y;
 
        while (ability > 0)
        {
            if (from.X != to.X)
            {
                x = isBackX ? x - Step : x + Step;
                ability-=Step;
            }

            if (from.Y != to.Y && ability > 0)
            {
                y = isBackY ? y - Step : y + Step;
                ability-=Step;
            }
        }
        
        return new Location(x, y);
    }
    
    private const int Step = 1;
}