using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.Models.SharedKernel;

public class Speed : ValueObject, ICorrectByMinMaxMax
{
    public static readonly int Min = 1;
    public static readonly int Max = 3;

    public Speed(int value)
    {
        Value = (this as ICorrectByMinMaxMax).IsCorrect(value, Min, Max)
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), $"Значение должно быть в пределах от {Min} до {Max})");
    }
    
    public int Value { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}