using RentACar.ML.Services;
using RentACar.ML.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ML services
builder.Services.AddSingleton<IMaintenancePredictionService, MaintenancePredictionService>();

// Register maintenance monitoring service
builder.Services.AddScoped<IMaintenanceMonitoringService, MaintenanceMonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run(); 