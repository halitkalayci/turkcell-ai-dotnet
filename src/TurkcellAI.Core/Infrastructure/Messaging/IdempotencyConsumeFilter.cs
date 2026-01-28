using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using TurkcellAI.Core.Application.Abstractions.Messaging;

namespace TurkcellAI.Core.Infrastructure.Messaging;

/// <summary>
/// Global consume filter that enforces idempotent handling using an IIdempotencyStore.
/// </summary>
/// <typeparam name="T">Message type</typeparam>
public class IdempotencyConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<IdempotencyConsumeFilter<T>> _logger;
    private readonly IIdempotencyStore _store;

    public IdempotencyConsumeFilter(ILogger<IdempotencyConsumeFilter<T>> logger, IIdempotencyStore store)
    {
        _logger = logger;
        _store = store;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var messageId = context.MessageId?.ToString();
        if (string.IsNullOrWhiteSpace(messageId))
        {
            await next.Send(context);
            return;
        }

        var consumerKey = context.Consumer?.GetType().Name
                          ?? context.ReceiveContext?.InputAddress?.ToString()
                          ?? typeof(T).Name;

        if (await _store.HasProcessedAsync(messageId!, consumerKey, context.CancellationToken))
        {
            _logger.LogInformation("Skipping duplicate message {MessageId} for {Consumer}", messageId, consumerKey);
            return; // ack without re-processing
        }

        await next.Send(context);

        // mark processed only after successful downstream handling
        await _store.MarkProcessedAsync(messageId!, consumerKey, context.CancellationToken);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("idempotencyConsumeFilter");
    }
}
