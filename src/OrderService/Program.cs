using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TurkcellAI.Core.Application.Abstractions;
using TurkcellAI.Core.Application.Behaviors;
using TurkcellAI.Core.Infrastructure;
using OrderService.Infrastructure.Messaging;
using TurkcellAI.Core.Application.DTOs;
using OrderService.Application.Ports;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using TurkcellAI.Core.Application.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (SQL Server)
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrderServiceDb")
    ));

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

// Authentication/Authorization
var authority = builder.Configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = false; // dev only
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.OrdersCreate, p => p.RequireClaim(ClaimNames.Permissions, "orders:order:create"));
    options.AddPolicy(PolicyNames.OrdersRead, p => p.RequireClaim(ClaimNames.Permissions, "orders:order:read"));
    options.AddPolicy(PolicyNames.OrdersUpdateStatus, p => p.RequireClaim(ClaimNames.Permissions, "orders:order:update_status"));
});

// Observability excluded in this implementation scope

var app = builder.Build();

// Middleware
app.UseCoreExceptionHandling();

// Auto-run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
