using Microsoft.Extensions.Logging;
using TurkcellAI.Core.Application.Abstractions.Messaging;

namespace TurkcellAI.Core.Infrastructure.Messaging;

/// <summary>
/// Placeholder for idempotent consume filter. Intentionally avoids MassTransit dependency in Core.
/// A concrete MassTransit-based filter should live in service projects.
/// </summary>
/// <typeparam name="T">Message type</typeparam>
public class IdempotencyConsumeFilter<T> where T : class
{
    public IdempotencyConsumeFilter(ILogger<IdempotencyConsumeFilter<T>> _logger, IIdempotencyStore _store) { }
}
