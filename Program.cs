using System;
using HotelComparer.Models;
using HotelComparer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using YourApiNamespace.Examples;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IAmadeusApiService, AmadeusApiService>();
builder.Services.AddScoped<IAmadeusApiTokenService, AmadeusApiTokenService>();
builder.Services.AddScoped<IHotelDataService, HotelDataService>();

// Register the Swagger generator, defining one or more Swagger documents
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Comparer API", Version = "v1" });
    c.ExampleFilters();
});

// Register Swagger examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<HotelOffersExample>(); // Updated this line

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Comparer API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
