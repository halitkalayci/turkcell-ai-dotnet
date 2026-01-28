using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TurkcellAI.Core.Application.Abstractions;
using TurkcellAI.Core.Application.Behaviors;
using TurkcellAI.Core.Infrastructure;
using OrderService.Infrastructure.Messaging;
using TurkcellAI.Core.Application.DTOs;
using OrderService.Application.Ports;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrderServiceDb"));

// Repositories and Unit of Work (Ports/Adapters)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<TurkcellAI.Core.Application.Abstractions.IUnitOfWork, UnitOfWork>();

// MediatR with pipeline behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Messaging (MassTransit + RabbitMQ + Outbox)
builder.Services.AddOrderServiceMessaging(builder.Configuration);

var app = builder.Build();

// Middleware
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
