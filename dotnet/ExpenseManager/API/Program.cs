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

// ✅ Add CORS policy
// WithOrigins("https://yourfrontend.com") → Restricts access to a specific frontend.
// AllowAnyMethod() → Allows all HTTP methods (GET, POST, PUT, DELETE, etc.).
// AllowAnyHeader() → Allows any request headers.
// AllowCredentials() → Enables sending credentials like cookies, tokens (⚠️ Only works with a specific origin, not *).
// AllowAnyOrigin() → Enables unrestricted access (only for local testing).

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://yourfrontend.com") // Replace with your frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // If authentication cookies/tokens are needed
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Use only for testing; NOT recommended for production
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

// Limit Request Size
// Prevent DoS attacks by limiting payload size.
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/options?view=aspnetcore-9.0
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000;
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
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Console.WriteLine(app.Environment.IsDevelopment().ToString());
Console.WriteLine($"Running in {builder.Environment.EnvironmentName} mode");

var IsDevelopment = app.Environment.IsDevelopment();

if (IsDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Use CORS Middleware before controllers
app.UseCors(IsDevelopment ? "AllowAll" : "AllowSpecificOrigins"); // Apply the selected CORS policy

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


// Prevent Cross-Site Scripting (XSS) & Clickjacking
// Use Content Security Policy (CSP) and X-Frame-Options:

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    await next();
});


app.Run();
