using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService.Application.Services;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using TurkcellAI.Core.Infrastructure;
using ProductService.Infrastructure.Messaging;
using TurkcellAI.Core.Application.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (SQL Server)
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ProductDb"),
        sql => sql.EnableRetryOnFailure()
    ));

// Add Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Application Services
builder.Services.AddScoped<IProductService, ProductService.Application.Services.ProductService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Messaging (MassTransit + RabbitMQ)
builder.Services.AddProductServiceMessaging(builder.Configuration);

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
    options.AddPolicy(PolicyNames.ProductsCreate, p => p.RequireClaim(ClaimNames.Permissions, "products:product:create"));
    options.AddPolicy(PolicyNames.ProductsRead, p => p.RequireClaim(ClaimNames.Permissions, "products:product:read"));
    options.AddPolicy(PolicyNames.ProductsUpdate, p => p.RequireClaim(ClaimNames.Permissions, "products:product:update"));
    options.AddPolicy(PolicyNames.ProductsDelete, p => p.RequireClaim(ClaimNames.Permissions, "products:product:delete"));
});

var app = builder.Build();

// Configure middleware
app.UseCoreExceptionHandling();

// Auto-run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
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
