using System;

namespace ProductService.Infrastructure.Messaging.Idempotency;

public class ProcessedMessage
{
    public Guid Id { get; set; }
    public string MessageId { get; set; } = default!;
    public string Consumer { get; set; } = default!;
    public DateTime ProcessedAtUtc { get; set; }
}
