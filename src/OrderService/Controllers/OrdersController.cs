using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Commands.UpdateOrderStatus;
using TurkcellAI.Core.Application.DTOs;
using OrderService.Application.DTOs;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Queries.GetOrders;
using OrderService.Domain.Enums;
using System.Diagnostics;

namespace OrderService.Controllers;

/// <summary>
/// Thin controller for order operations.
/// Delegates all business logic to MediatR command and query handlers.
/// </summary>
[ApiController]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(OrderListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] Guid? customerId = null)
    {
        if (pageNumber < 1)
        {
            return BadRequest(CreateErrorResponse(TurkcellAI.Core.Application.Enums.ErrorCode.INVALID_PARAMETER, "pageNumber must be at least 1"));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(CreateErrorResponse(TurkcellAI.Core.Application.Enums.ErrorCode.INVALID_PARAMETER, "pageSize must be between 1 and 100"));
        }

        var query = new GetOrdersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Status = status,
            CustomerId = customerId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
    {
        var query = new GetOrderByIdQuery(orderId);
        var order = await _mediator.Send(query);
        
        if (order == null)
        {
            return NotFound(CreateErrorResponse(TurkcellAI.Core.Application.Enums.ErrorCode.NOT_FOUND, $"Order with ID {orderId} not found"));
        }

        return Ok(order);
    }

    [HttpPatch("{orderId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderStatusCommand command)
    {
        command.OrderId = orderId;
        await _mediator.Send(command);
        
        return NoContent();
    }

    private TurkcellAI.Core.Application.DTOs.ErrorResponse CreateErrorResponse(TurkcellAI.Core.Application.Enums.ErrorCode code, string message)
    {
        var traceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return new TurkcellAI.Core.Application.DTOs.ErrorResponse
        {
            TraceId = traceId,
            Code = code,
            Message = message
        };
    }
}
