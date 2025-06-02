using System.Threading;
using System.Threading.Tasks;
using DeliveryApp.Core.Domain.Models.SharedKernel;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    Task<Location> GetLocation(string address, CancellationToken cancellationToken);
}