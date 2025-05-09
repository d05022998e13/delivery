using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.SharedKernel.Interfaces;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

public class Location : ValueObject, ICorrectByMinMaxMax
{
    public static readonly int Min = 1;
    public static readonly int Max = 10;

    public Location(int x, int y)
    {
        X = (this as ICorrectByMinMaxMax).IsCorrect(x, Min, Max)
            ? x
            : throw new ArgumentOutOfRangeException(nameof(x), $"Значение X должно быть в пределах от {Min} до {Max})");
        
        Y = (this as ICorrectByMinMaxMax).IsCorrect(y, Min, Max)
            ? y
            : throw new ArgumentOutOfRangeException(nameof(y), $"Значение Y должно быть в пределах от {Min} до {Max})");
    }
    
    public int X { get; }

    public int Y { get; }
    
    public int DistanceTo(Location other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    public static Location Random()
    {
        var random = new Random();
        
        return new Location(random.Next(Min, Max), random.Next(Min, Max));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}