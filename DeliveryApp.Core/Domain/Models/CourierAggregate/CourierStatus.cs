using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate;

public class CourierStatus : ValueObject
{
    public static CourierStatus Free => new(nameof(Free).ToLowerInvariant());
    public static CourierStatus Busy => new(nameof(Busy).ToLowerInvariant());
    
    private CourierStatus(string name)
    {
        Name = name;
    }
    
    public string Name { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

}