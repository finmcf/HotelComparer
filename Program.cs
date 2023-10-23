using HotelComparer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using Microsoft.EntityFrameworkCore;
using HotelComparer.Data;
using HotelComparer.Middleware;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Register DbContext with a connection string from configuration
var connectionString = builder.Configuration["Database:ConnectionString"];

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseMySql(connectionString,
    new MySqlServerVersion(new Version(8, 0, 26))));

builder.Services.AddScoped<IAmadeusApiService, AmadeusApiService>();
builder.Services.AddScoped<IAmadeusApiTokenService, AmadeusApiTokenService>();
builder.Services.AddScoped<IHotelDataService, HotelDataService>();

// Register the Swagger generator and add API key support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Comparer API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key Authorization header using the ApiKey scheme.",
        In = ParameterLocation.Header,
        Name = "ApiKey",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });

    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<HotelOfferDataExample>();

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
