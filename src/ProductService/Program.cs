using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductService.Application.Services;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using TurkcellAI.Core.Infrastructure;
using ProductService.Infrastructure.Messaging;

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
app.UseAuthorization();
app.MapControllers();

app.Run();
