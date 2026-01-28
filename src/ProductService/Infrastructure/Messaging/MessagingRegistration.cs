using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Infrastructure.Messaging;
using TurkcellAI.Core.Application.Abstractions.Messaging;
using ProductService.Infrastructure.Messaging.Idempotency;

namespace ProductService.Infrastructure.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddProductServiceMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MessagingOptions>()
            .Bind(configuration.GetSection("Messaging"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();
        services.AddScoped(typeof(IdempotencyConsumeFilter<>));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>(cfg =>
            {
                cfg.UseConcurrentMessageLimit(8);
            });

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<MessagingOptions>>().Value;

                if (options.Enabled && !string.IsNullOrWhiteSpace(options.Host))
                {
                    cfg.Host(options.Host, options.VirtualHost ?? "/", h =>
                    {
                        if (!string.IsNullOrWhiteSpace(options.Username)) h.Username(options.Username);
                        if (!string.IsNullOrWhiteSpace(options.Password)) h.Password(options.Password);
                    });
                }

                cfg.PrefetchCount = (ushort)Math.Max(1, options.Prefetch);

                cfg.UseMessageRetry(r =>
                    r.Exponential(
                        options.Retry.MaxAttempts,
                        options.Retry.MinBackoff,
                        options.Retry.MaxBackoff,
                        options.Retry.IntervalFactor));

                // Apply idempotency filter globally to all message types
                cfg.UseConsumeFilter(typeof(IdempotencyConsumeFilter<>), context);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
