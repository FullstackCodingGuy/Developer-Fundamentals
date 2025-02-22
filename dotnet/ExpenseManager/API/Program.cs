using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Application.Services;
using ExpenseManager.Infrastructure.Repositories;
using ExpenseManager.Infrastructure.Persistence;
using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCaching();

// ✅ Ensure correct access to configuration
var configuration = builder.Configuration;

// ✅ Use proper configuration access for connection string
var connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? "Data Source=expensemanager.db";

builder.Services.AddDbContext<ExpenseDbContext>(options =>
    options.UseSqlite(connectionString));


// Use Rate Limiting
// Prevent API abuse by implementing rate limiting
// Add Rate Limiting Middleware
// Now, each client IP gets 10 requests per minute, with a queue of 2 extra requests.

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // Return 429 when limit is exceeded

    // ✅ Explicitly specify type <string> for AddPolicy
    options.AddPolicy<string>("fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "default",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // Allow 10 requests
                Window = TimeSpan.FromMinutes(1), // Per 1-minute window
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2 // Allow 2 extra requests in queue
            }));
});


// Add services to the container
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

// Use System.Text.Json instead of Newtonsoft.Json for better performance.

// Enable reference handling and lower casing for smaller responses:
// Explanation
// JsonNamingPolicy.CamelCase → Converts property names to camelCase (recommended for APIs).
// ReferenceHandler.Preserve → Prevents circular reference issues when serializing related entities.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable Compression to reduce payload size
builder.Services.AddResponseCompression();

var app = builder.Build();

// Console.WriteLine(app.Environment.IsDevelopment().ToString());
Console.WriteLine($"Running in {builder.Environment.EnvironmentName} mode");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Console.WriteLine(app.Environment.IsDevelopment().ToString());
app.UseResponseCompression();

// use rate limiter
app.UseRateLimiter();

// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
