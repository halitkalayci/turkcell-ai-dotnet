using MassTransit;
using Microsoft.Extensions.Logging;
using TurkcellAI.Contracts.Orders.V1;

namespace ProductService.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "Consumed OrderCreated: OrderId={OrderId}, CustomerId={CustomerId}, Total={Total} {Currency}, MessageId={MessageId}",
            msg.OrderId, msg.CustomerId, msg.TotalAmount, msg.Currency, context.MessageId);

        // NOTE: In a future batch, persist idempotency records (Inbox) and
        // update read models or trigger domain processes as required.
        return Task.CompletedTask;
    }
}
