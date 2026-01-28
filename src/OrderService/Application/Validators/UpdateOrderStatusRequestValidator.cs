using FluentValidation;
using OrderService.Application.DTOs;
using OrderService.Domain.Enums;

namespace OrderService.Application.Validators;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .Must(BeAValidStatus)
            .WithMessage("Status must be one of: Pending, Confirmed, Shipped, Delivered, Cancelled");
    }

    private bool BeAValidStatus(string status)
    {
        return Enum.TryParse<OrderStatus>(status, true, out _);
    }
}
