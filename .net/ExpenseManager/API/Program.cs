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

var builder = WebApplication.CreateBuilder(args);

// ✅ Ensure correct access to configuration
var configuration = builder.Configuration;

// ✅ Use proper configuration access for connection string
var connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? "Data Source=expensemanager.db";

builder.Services.AddDbContext<ExpenseDbContext>(options =>
    options.UseSqlite(connectionString));


// Add services to the container
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Console.WriteLine(app.Environment.IsDevelopment().ToString());
Console.WriteLine($"Running in {builder.Environment.EnvironmentName} mode");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


// Ensure Database is Created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
