using HotelComparer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddMemoryCache(); // Existing line

// Existing service registrations
builder.Services.AddScoped<IAmadeusApiService, AmadeusApiService>();
builder.Services.AddScoped<IAmadeusApiTokenService, AmadeusApiTokenService>();

// New service registration
builder.Services.AddScoped<IHotelDataService, HotelDataService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
