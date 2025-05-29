using DeliveryApp.Core.Ports;
using GeoApp.Api;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Options;
using Location = DeliveryApp.Core.Domain.Models.SharedKernel.Location;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.Geo;

public class GeoClient(IOptions<Settings> options) : IGeoClient
{
    private readonly MethodConfig _methodConfig = new()
    {
        Names = { MethodName.Default },
        RetryPolicy = new RetryPolicy
        {
            MaxAttempts = 5,
            InitialBackoff = TimeSpan.FromSeconds(1),
            MaxBackoff = TimeSpan.FromSeconds(5),
            BackoffMultiplier = 1.5,
            RetryableStatusCodes = { StatusCode.Unavailable }
        }
    };

    private readonly SocketsHttpHandler _socketsHttpHandler = new()
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };
    private readonly string _url = options.Value.GeoServiceGrpcHost;
    
    public async Task<Location> GetLocation(string street, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
        {
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
        });
        
        var client = new GeoApp.Api.Geo.GeoClient(channel);
        var reply = await client.GetGeolocationAsync(
            new GetGeolocationRequest
        {
            Street = street
        },
        null,
        DateTime.UtcNow.AddSeconds(5),
        cancellationToken);
        
        return new Location(reply.Location.X, reply.Location.Y);
    }
}