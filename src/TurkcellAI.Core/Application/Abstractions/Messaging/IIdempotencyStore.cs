using System.Threading;
using System.Threading.Tasks;

namespace TurkcellAI.Core.Application.Abstractions.Messaging
{
    /// <summary>
    /// Abstraction for consumer-side idempotency (Inbox) to deduplicate message handling.
    /// </summary>
    public interface IIdempotencyStore
    {
        Task<bool> HasProcessedAsync(string messageId, string consumerKey, CancellationToken cancellationToken = default);
        Task MarkProcessedAsync(string messageId, string consumerKey, CancellationToken cancellationToken = default);
    }
}
