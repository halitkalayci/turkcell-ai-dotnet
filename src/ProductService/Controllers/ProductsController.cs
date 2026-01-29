using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurkcellAI.Core.Application.Authorization;
using ProductService.Application.DTOs;
using TurkcellAI.Core.Application.DTOs;
using TurkcellAI.Core.Application.Enums;
using ProductService.Application.Services;

namespace ProductService.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }


    /// <summary>
    /// Test endpoint: No-auth, returns a dummy product list for health check.
    /// </summary>
    [HttpGet("test-noauth")]
    [AllowAnonymous]
    public IActionResult TestNoAuth()
    {
        return Ok(new[] { new { ProductId = Guid.Empty, Name = "Test", Message = "ProductService no-auth endpoint works" } });
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.ProductsCreate)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errorResponse = new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.VALIDATION_ERROR,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new TurkcellAI.Core.Application.DTOs.ValidationError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList()
            };
            return BadRequest(errorResponse);
        }

        var product = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.ProductsRead)]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        if (pageNumber < 1)
        {
            return BadRequest(new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.INVALID_PARAMETER,
                Message = "Page number must be greater than 0"
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.INVALID_PARAMETER,
                Message = "Page size must be between 1 and 100"
            });
        }

        var result = await _productService.GetAllAsync(pageNumber, pageSize, category, minPrice, maxPrice);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyNames.ProductsRead)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound(new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.NOT_FOUND,
                Message = $"Product with ID {id} not found"
            });
        }

        return Ok(product);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyNames.ProductsUpdate)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request)
    {
        var validationResult = await _updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errorResponse = new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.VALIDATION_ERROR,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => new TurkcellAI.Core.Application.DTOs.ValidationError
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList()
            };
            return BadRequest(errorResponse);
        }

        var product = await _productService.UpdateAsync(id, request);
        if (product == null)
        {
            return NotFound(new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.NOT_FOUND,
                Message = $"Product with ID {id} not found"
            });
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = PolicyNames.ProductsDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(TurkcellAI.Core.Application.DTOs.ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new TurkcellAI.Core.Application.DTOs.ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Code = ErrorCode.NOT_FOUND,
                Message = $"Product with ID {id} not found"
            });
        }

        return NoContent();
    }
}
