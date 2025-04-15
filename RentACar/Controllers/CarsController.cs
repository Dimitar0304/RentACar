using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Models.CategoryDto;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Vehicle;
using System.Threading.Tasks;

namespace RentACar.Controllers
{
    public class CarsController : Controller
    {
        private readonly RentCarDbContext _context;

        public CarsController(RentCarDbContext context)
        {
            _context = context;
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
                        // Add other CategoryViewModel properties as needed
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