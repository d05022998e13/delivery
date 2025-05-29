using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.Jobs;

[DisallowConcurrentExecution]
public class CreateOrderJob(IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await mediator.Send(new CreateOrderCommand(Guid.NewGuid(), "Тестировочная"));
    }
}