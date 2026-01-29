using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TurkcellAI.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient to talk to Keycloak (IdP)
var authority = builder.Configuration["Jwt:Authority"] ?? "http://localhost:8080/realms/turkcell-ai";
builder.Services.AddHttpClient("Idp", client =>
{
    // BaseAddress not strictly necessary since we use absolute URLs
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.UseCoreExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
