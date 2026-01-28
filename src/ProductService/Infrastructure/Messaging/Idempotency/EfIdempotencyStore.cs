using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Data;
using TurkcellAI.Core.Application.Abstractions.Messaging;

namespace ProductService.Infrastructure.Messaging.Idempotency;

public class EfIdempotencyStore : IIdempotencyStore
{
    private readonly ProductDbContext _db;

    public EfIdempotencyStore(ProductDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasProcessedAsync(string messageId, string consumerKey, CancellationToken cancellationToken = default)
    {
        return await _db.ProcessedMessages.AnyAsync(x => x.MessageId == messageId && x.Consumer == consumerKey, cancellationToken);
    }

    public async Task MarkProcessedAsync(string messageId, string consumerKey, CancellationToken cancellationToken = default)
    {
        var entity = new ProcessedMessage
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            Consumer = consumerKey,
            ProcessedAtUtc = DateTime.UtcNow
        };

        _db.ProcessedMessages.Add(entity);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Unique index guards duplicates; treat as already processed in race conditions.
        }
    }
}
