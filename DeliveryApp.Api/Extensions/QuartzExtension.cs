using DeliveryApp.Api.Adapters.Jobs;
using Quartz;

namespace DeliveryApp.Api.Extensions;

public static class QuartzExtension
{
    public static IServiceCollection ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var createOrderJobJobKey = new JobKey(nameof(CreateOrderJob));
            var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
            var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));

            configure
                .AddJob<CreateOrderJob>(createOrderJobJobKey)
                .AddTrigger(
                    trigger => trigger
                        .ForJob(createOrderJobJobKey)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1)));
                
            configure
                .AddJob<AssignOrdersJob>(assignOrdersJobKey)
                .AddTrigger(
                    trigger => trigger
                        .ForJob(assignOrdersJobKey)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).RepeatForever()))
                
                .AddJob<MoveCouriersJob>(moveCouriersJobKey)
                .AddTrigger(
                    trigger => trigger
                        .ForJob(moveCouriersJobKey)
                        .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(2).RepeatForever()));
        });
        
        services.AddQuartzHostedService();
        
        return services;
    }
}