using System;

namespace TurkcellAI.Core.Application.Abstractions.Messaging
{
    /// <summary>
    /// Base contract for all integration events published across services.
    /// Keep minimal surface and avoid framework dependencies.
    /// </summary>
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        DateTime OccurredAtUtc { get; }
        string Source { get; }
        string Version { get; }
    }
}
