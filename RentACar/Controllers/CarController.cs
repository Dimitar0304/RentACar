using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data;
using RentACar.Core.Models.CategoryDto;

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
                    Category = c.Category.Name,
                    Hp = c.Hp,
                    ImageUrl = c.ImageUrl,
                    PricePerDay = c.PricePerDay
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;

            return View(cars);
        }

        public async Task<IActionResult> Search(string make, string model, int? maxPrice)
        {
            var query = _context.Cars.AsQueryable();

            // Apply make filter
            if (!string.IsNullOrEmpty(make))
            {
                query = query.Where(c => c.Make.Contains(make));
            }

            // Apply model filter
            if (!string.IsNullOrEmpty(model))
            {
                query = query.Where(c => c.Model.Contains(model));
            }

            // Apply price filter
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);
            }

            var cars = await query
                .Include(c => c.Category)
                .Select(c => new CarViewModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Hp = c.Hp,
                    IsRented = c.IsRented,
                    CategoryId = c.CategoryId,
                    Mileage = c.Mileage,
                    ImageUrl = c.ImageUrl,
                    PricePerDay = c.PricePerDay,
                    Categories = new[] { new CategoryViewModel 
                    { 
                        Id = c.Category.Id,
                        Name = c.Category.Name
                    }}
                }).ToListAsync();

            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarViewModel
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Hp = car.Hp,
                IsRented = car.IsRented,
                CategoryId = car.CategoryId,
                Mileage = car.Mileage,
                ImageUrl = car.ImageUrl,
                PricePerDay = car.PricePerDay,
                Categories = new[] { new CategoryViewModel 
                { 
                    Id = car.Category.Id,
                    Name = car.Category.Name
                }}
            };

            return View(viewModel);
        }
    }
}
