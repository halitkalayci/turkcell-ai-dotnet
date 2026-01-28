using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Services;
using OrderService.Domain.Enums;
using System.Diagnostics;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IValidator<CreateOrderRequest> _createOrderValidator;
    private readonly IValidator<UpdateOrderStatusRequest> _updateStatusValidator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        IValidator<CreateOrderRequest> createOrderValidator,
        IValidator<UpdateOrderStatusRequest> updateStatusValidator,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _createOrderValidator = createOrderValidator;
        _updateStatusValidator = updateStatusValidator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var validationResult = await _createOrderValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var order = await _orderService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] Guid? customerId = null)
    {
        if (pageNumber < 1)
        {
            return BadRequest(CreateErrorResponse("INVALID_PARAMETER", "pageNumber must be at least 1"));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(CreateErrorResponse("INVALID_PARAMETER", "pageSize must be between 1 and 100"));
        }

        OrderStatus? orderStatus = null;
        if (!string.IsNullOrEmpty(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                return BadRequest(CreateErrorResponse("INVALID_PARAMETER", 
                    "status must be one of: Pending, Confirmed, Shipped, Delivered, Cancelled"));
            }
            orderStatus = parsedStatus;
        }

        var result = await _orderService.GetOrdersAsync(pageNumber, pageSize, orderStatus, customerId);
        return Ok(result);
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        
        if (order == null)
        {
            return NotFound(CreateErrorResponse("ORDER_NOT_FOUND", $"Order with ID {orderId} not found"));
        }

        return Ok(order);
    }

    [HttpPatch("{orderId}/status")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var validationResult = await _updateStatusValidator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var order = await _orderService.UpdateOrderStatusAsync(orderId, request);
        
        if (order == null)
        {
            return NotFound(CreateErrorResponse("ORDER_NOT_FOUND", $"Order with ID {orderId} not found"));
        }

        return Ok(order);
    }

    private ErrorResponse CreateErrorResponse(string code, string message)
    {
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return new ErrorResponse
        {
            TraceId = traceId,
            Code = code,
            Message = message
        };
    }
}
