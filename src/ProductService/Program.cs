using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Services;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using TurkcellAI.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with In-Memory database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));

// Add Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Application Services
builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure middleware
app.UseCoreExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
