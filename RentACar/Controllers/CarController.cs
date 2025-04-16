using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data;

namespace RentACar.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService service;
        private readonly RentCarDbContext _context;
        
        public CarController(ICarService carService, RentCarDbContext context)
        {
            service = carService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1)
        {
            const int ItemsPerPage = 7;
            
            var query = _context.Cars
                .OrderByDescending(c => c.Id);

            var cars = await query
                .Skip((page - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(c => new CarAllViewModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Hp = c.Hp,
                    ImageUrl = c.ImageUrl,
                    PricePerDay = c.PricePerDay
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;

            return View(cars);
        }
    }
}
