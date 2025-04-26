namespace DeliveryApp.Core.Domain.Models.SharedKernel;

public interface ICorrectByMinMaxMax
{ 
    bool IsCorrect(int value, int min, int max) => value >= min && value <= max;
}