using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public class Courier : Aggregate<Guid>
{
    public string Name { get;  private set; }
    
    public Location Location { get; private set; }
    
    public CourierStatus Status { get; private set; }
    
    public Transport Transport { get; private set; }

    public static Courier Create(string name, string transportName, int transportSpeed, Location location)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Не указано имя курьера.", nameof(name));
        
        if (location == null) throw new ArgumentException("Не указано местоположение.", nameof(location));
        
        var transport = new Transport(transportName, transportSpeed);

        return new Courier
        {
            Id = Guid.NewGuid(),
            Name = name,
            Transport = transport,
            Location = location,
            Status = CourierStatus.Free
        };
    }

    public void SetBusy()
    {
        ChangeStatus(CourierStatus.Busy);
    }
    
    public void SetFree()
    {
        ChangeStatus(CourierStatus.Free);
    }

    public float CalculateTimeToLocation(Location location)
    {
        if (location == null) throw new ArgumentException("Не указано местоположение.", nameof(location));

        var distance = Location.DistanceTo(location);
        
        return distance > 0
            ? distance > Transport.Speed.Value
                ? (float)distance / Transport.Speed.Value
                : 1
            : 0;
    }

    public void Move(Location location)
    {
        if (location == null) throw new ArgumentException("Не указано местоположение.", nameof(location));
        
        Location = Transport.Move(Location, location);
    }

    private void ChangeStatus(CourierStatus status)
    {
        if (status == null) throw new ArgumentException("Не указан статус.", nameof(status));

        if (Status == status) throw new Exception($"Курьер уже находится в статусе \"{Status}\"");
        
        Status = status;
    }
}