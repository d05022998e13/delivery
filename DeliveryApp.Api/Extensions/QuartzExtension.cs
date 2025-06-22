using DeliveryApp.Api.Adapters.Jobs;
using DeliveryApp.Infrastructure.Adapters.Postgres.Outbox;
using Quartz;

namespace DeliveryApp.Api.Extensions;

public static class QuartzExtension
{
    public static IServiceCollection ConfigureQuartz(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            configure.OutboxMessagesJob();
            
            // configure.CreateOrderJob();
            configure.AssignOrdersJob();
            configure.MoveCouriersJob();
        });
        
        services.AddQuartzHostedService();
        
        return services;
    }
    
    private static void OutboxMessagesJob(this IServiceCollectionQuartzConfigurator configure)
    {
        var outboxMessagesJob = new JobKey(nameof(OutboxMessagesJob));
        
        configure
            .AddJob<OutboxMessagesJob>(outboxMessagesJob)
            .AddTrigger(
                trigger => trigger
                    .ForJob(outboxMessagesJob)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(3).RepeatForever()));
    }

    private static void CreateOrderJob(this IServiceCollectionQuartzConfigurator configure)
    {
        var createOrderJobJobKey = new JobKey(nameof(CreateOrderJob));
        
        configure
            .AddJob<CreateOrderJob>(createOrderJobJobKey)
            .AddTrigger(
                trigger => trigger
                    .ForJob(createOrderJobJobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1)));
    }
    
    private static void AssignOrdersJob(this IServiceCollectionQuartzConfigurator configure)
    {
        var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
        
        configure
            .AddJob<AssignOrdersJob>(assignOrdersJobKey)
            .AddTrigger(
                trigger => trigger
                    .ForJob(assignOrdersJobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(1).RepeatForever()));
    }
    
    private static void MoveCouriersJob(this IServiceCollectionQuartzConfigurator configure)
    {
        var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
        
        configure 
            .AddJob<MoveCouriersJob>(moveCouriersJobKey)
            .AddTrigger(
                trigger => trigger
                    .ForJob(moveCouriersJobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(2).RepeatForever()));
    }
}