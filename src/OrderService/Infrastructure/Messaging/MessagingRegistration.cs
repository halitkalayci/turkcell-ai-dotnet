using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderService.Infrastructure.Data;
using TurkcellAI.Core.Application.DTOs;

namespace OrderService.Infrastructure.Messaging;

public static class MessagingRegistration
{
    public static IServiceCollection AddOrderServiceMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MessagingOptions>()
            .Bind(configuration.GetSection("Messaging"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // If messaging is disabled, configure in-memory bus so handlers can still publish without external broker
        var messagingEnabled = configuration.GetSection("Messaging").GetValue<bool>("Enabled");
        if (!messagingEnabled)
        {
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        services.AddMassTransit(x =>
        {
            // Outbox: keep messages until the DB transaction commits
            x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
            {
                o.UseBusOutbox();
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
                        TimeSpan.FromSeconds(options.Retry.IntervalFactor)));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
