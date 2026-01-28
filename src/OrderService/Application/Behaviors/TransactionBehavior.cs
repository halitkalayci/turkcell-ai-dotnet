using MediatR;
using OrderService.Application.Ports;

namespace OrderService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that wraps command execution in a database transaction.
/// Ensures all changes within a command are committed atomically.
/// Only applies to commands that modify state.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only apply transactions to commands (requests that end with "Command")
        var isCommand = typeof(TRequest).Name.EndsWith("Command");

        if (!isCommand)
        {
            return await next();
        }

        // Execute the command handler
        var response = await next();

        // Commit changes to the database
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
