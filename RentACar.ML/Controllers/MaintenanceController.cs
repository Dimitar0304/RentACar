using Microsoft.AspNetCore.Mvc;
using RentACar.ML.Services.Interfaces;

namespace RentACar.ML.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceMonitoringService _maintenanceService;

        public MaintenanceController(IMaintenanceMonitoringService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetMaintenanceAlerts()
        {
            var alerts = await _maintenanceService.CheckMaintenanceStatusAsync();
            return Ok(alerts);
        }

        [HttpGet("car/{id}")]
        public async Task<IActionResult> GetCarMaintenanceStatus(int id)
        {
            try
            {
                var status = await _maintenanceService.GetCarMaintenanceStatusAsync(id);
                return Ok(status);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
} 