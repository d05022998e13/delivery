namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;

public sealed class GetBusyCouriersResponse
{
    public GetBusyCouriersResponse(List<Courier> couriers)
    {
        Couriers.AddRange(couriers);
    }

    public List<Courier> Couriers { get; set; } = new();
}

public sealed class Courier
{
    /// <summary>
    ///     Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Имя
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Геопозиция (X,Y)
    /// </summary>
    public Location Location { get; set; }
}

public sealed class Location
{
    /// <summary>
    ///     Горизонталь
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     Вертикаль
    /// </summary>
    public int Y { get; set; }
}