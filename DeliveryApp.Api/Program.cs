using System.Reflection;
using DeliveryApp.Api;
using DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;
using DeliveryApp.Api.Extensions;
using DeliveryApp.Core.Application.DomainEventHandlers;
using DeliveryApp.Core.Application.UseCases.Commands.AssignOrder;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;
using DeliveryApp.Core.Domain.Models.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Domain.Services.DispatchService;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Grpc.Geo;
using DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenApi.Formatters;
using Primitives;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();
var connectionString = builder.Configuration["CONNECTION_STRING"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString, sqlOptions => sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"));
        options.EnableSensitiveDataLogging();
    }
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Domain Services
builder.Services.AddScoped<IDispatchService, DispatchService>();

// Repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();

builder.Services.AddScoped<IQueryDbContext, ApplicationDbContext>();

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Commands
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, bool>, CreateOrderHandler>();
builder.Services.AddScoped<IRequestHandler<MoveCouriersCommand, bool>, MoveCouriersHandler>();
builder.Services.AddScoped<IRequestHandler<AssignOrderCommand, bool>, AssignOrderHandler>();

// Queries
builder.Services.AddScoped<IRequestHandler<GetBusyCouriersQuery, GetBusyCouriersResponse>, GetBusyCouriersHandler>();
builder.Services.AddScoped<IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>, GetNotCompletedOrdersHandler>();

// gRPC
builder.Services.AddTransient<IGeoClient, GeoClient>();

// Message Broker Consumer
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHostedService<ConsumerService>();

// Domain Event Handlers
builder.Services.AddTransient<INotificationHandler<OrderStatusChangedDomainEvent>, OrderStatusChangedDomainEventHandler>();

// Message Broker Producer
builder.Services.AddTransient<IMessageBusProducer, ProducerService>();

// Jobs
builder.Services.ConfigureQuartz();

builder.Services.ConfigureSwagger();

builder.Services.AddControllers(options => { options.InputFormatters.Insert(0, new InputFormatterStream()); })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.Converters.Add(new StringEnumConverter
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        });
    });

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

//Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //db.Database.Migrate();
}

app.UseConfiguredSwagger();

app.UseCors();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();