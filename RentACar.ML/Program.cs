using RentACar.ML.Services;
using RentACar.ML.Services.Interfaces;
using RentACar.Infrastructure.Data;
using RentACar.Core.Services;
using RentACar.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database context
builder.Services.AddDbContext<RentCarDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ML services
builder.Services.AddSingleton<IMaintenancePredictionService, MaintenancePredictionService>();

// Register maintenance monitoring service
builder.Services.AddScoped<IMaintenanceMonitoringService, MaintenanceMonitoringService>();

// Register car metrics service
builder.Services.AddScoped<ICarMetricsService, CarMetricsService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Initialize the database
await DatabaseInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run(); 