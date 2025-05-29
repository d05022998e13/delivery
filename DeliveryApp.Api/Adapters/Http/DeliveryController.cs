using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.Http;

/// <inheritdoc />
public class DeliveryController(IMediator mediator) : DefaultApiController
{
    public override Task<IActionResult> CreateCourier(NewCourier newCourier)
    {
        throw new NotImplementedException();
    }

    public override async Task<IActionResult> CreateOrder()
    {
        var response = await mediator.Send(new CreateOrderCommand(Guid.NewGuid(), "test street"));
        if (response) return Ok();
        return BadRequest();
    }

    public override async Task<IActionResult> GetCouriers() => Ok(await mediator.Send(new GetBusyCouriersQuery()));

    public override async Task<IActionResult> GetOrders() => Ok(await mediator.Send(new GetNotCompletedOrdersQuery()));
}