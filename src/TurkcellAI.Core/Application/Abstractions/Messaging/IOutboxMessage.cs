using System;

namespace TurkcellAI.Core.Application.Abstractions.Messaging
{
    /// <summary>
    /// Minimal abstraction representing a stored outbox message for reliable publishing.
    /// Implementations live in service Infrastructure and may add fields.
    /// </summary>
    public interface IOutboxMessage
    {
        Guid Id { get; }
        string Type { get; }
        string Payload { get; }
        string? HeadersJson { get; }
        DateTime OccurredAtUtc { get; }
        DateTime? ProcessedAtUtc { get; }
        int AttemptCount { get; }
    }
}
