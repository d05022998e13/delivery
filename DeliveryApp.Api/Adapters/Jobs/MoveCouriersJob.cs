using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.Jobs;

public class MoveCouriersJob(IMediator mediator) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await mediator.Send(new MoveCouriersCommand());
    }
}