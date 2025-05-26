using System.Reflection;
using Microsoft.OpenApi.Models;
using OpenApi.Filters;
using OpenApi.OpenApi;

namespace DeliveryApp.Api.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("1.0.0", new OpenApiInfo
            {
                Title = "Delivery Service",
                Description = "Отвечает за доставку заказа"
            });
            options.CustomSchemaIds(type => type.FriendlyId(true));
            options.IncludeXmlComments(
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
            options.DocumentFilter<BasePathFilter>("");
            options.OperationFilter<GeneratePathParamsValidationFilter>();
        });
        
        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    public static WebApplication UseConfiguredSwagger(this WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; })
            .UseSwaggerUI(options =>
            {
                options.RoutePrefix = "openapi";
                options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Basket Service");
                options.RoutePrefix = string.Empty;
                options.SwaggerEndpoint("/openapi-original.json", "Swagger Basket Service");
            });
        
        return app;
    }
}