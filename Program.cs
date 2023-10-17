using HotelComparer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using HotelComparer.Examples;
using System;
using Microsoft.EntityFrameworkCore; // For EF Core
using HotelComparer.Data; // For DB context
using HotelComparer.Middleware; // For custom middleware
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // For MySQL server version specification

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Register DbContext with a connection string for MySQL
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseMySql("Server=localhost;Database=HotelComparerApiKeys;User=root;Password=Waterbottle99*;",
    new MySqlServerVersion(new Version(8, 1, 0)))); // Specify the MySQL version here

builder.Services.AddScoped<IAmadeusApiService, AmadeusApiService>();
builder.Services.AddScoped<IAmadeusApiTokenService, AmadeusApiTokenService>();
builder.Services.AddScoped<IHotelDataService, HotelDataService>();

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Comparer API", Version = "v1" });
    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<HotelOffersExample>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Comparer API v1");
    });
}

app.UseHttpsRedirection();

// Use middleware for API key and permissions checking
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();
app.Run();
