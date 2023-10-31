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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Register the IHttpClientFactory
builder.Services.AddHttpClient();

// Use the same connection string retrieval method as before
var connectionString = builder.Configuration["Database:ConnectionString"];
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26))));

// Register your services here as in the old version
builder.Services.AddScoped<IAmadeusApiService, AmadeusApiService>();
builder.Services.AddScoped<IAmadeusApiTokenService, AmadeusApiTokenService>();
builder.Services.AddScoped<IHotelDataService, HotelDataService>();
builder.Services.AddScoped<IAmadeusAutocompleteService, AmadeusAutocompleteService>();

// Swagger configuration as before
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

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
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

// API key middleware as before
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
