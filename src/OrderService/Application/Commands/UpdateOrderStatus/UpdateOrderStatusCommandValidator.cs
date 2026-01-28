using FluentValidation;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands.UpdateOrderStatus;

/// <summary>
/// Validator for UpdateOrderStatusCommand.
/// Ensures status update requests are valid.
/// </summary>
public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid order status");
    }
}
