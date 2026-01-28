using FluentValidation;

namespace OrderService.Application.Commands.CreateOrder;

/// <summary>
/// Validator for CreateOrderCommand.
/// Ensures order creation requests contain valid data.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            item.RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required")
                .MaximumLength(200)
                .WithMessage("Product name cannot exceed 200 characters");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero");
        });
    }
}
