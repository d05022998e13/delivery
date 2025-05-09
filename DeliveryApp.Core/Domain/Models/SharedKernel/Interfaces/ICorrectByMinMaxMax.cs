namespace DeliveryApp.Core.Domain.Models.SharedKernel.Interfaces;

public interface ICorrectByMinMaxMax
{ 
    bool IsCorrect(int value, int min, int max) => value >= min && value <= max;
}