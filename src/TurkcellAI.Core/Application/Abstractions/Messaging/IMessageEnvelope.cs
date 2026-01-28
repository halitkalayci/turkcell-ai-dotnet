using System;
using System.Collections.Generic;

namespace TurkcellAI.Core.Application.Abstractions.Messaging
{
    /// <summary>
    /// Standard envelope abstraction for integration events with common headers.
    /// </summary>
    /// <typeparam name="TEvent">The event payload type.</typeparam>
    public interface IMessageEnvelope<out TEvent> where TEvent : IIntegrationEvent
    {
        TEvent Payload { get; }

        string MessageId { get; }
        string? CorrelationId { get; }
        string? CausationId { get; }
        string? TraceId { get; }

        string Type { get; }
        string Version { get; }
        DateTime OccurredAtUtc { get; }
        string Source { get; }
        string? TenantId { get; }

        IReadOnlyDictionary<string, string> Headers { get; }
    }
}
