using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using RentACar.Infrastructure.Data;
using RentACar.Core.Models.CategoryDto;
using Microsoft.AspNetCore.Authorization;
using System.Configuration;
using System.Security.Claims;
using RentACar.Core.Models.RentBillDto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Linq;

namespace RentACar.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService carService;
        private readonly IRentBillService _rentBillService;
        private readonly RentCarDbContext _context;
        
        public CarController(ICarService _carService, RentCarDbContext context, IRentBillService rentBillService)
        {
            carService = _carService;
            _context = context;
            _rentBillService = rentBillService;
        }

        [HttpGet]
        public async Task<IActionResult> All(int page = 1)
        {
            const int ItemsPerPage = 7;

            var query = await carService.GetAllCarsAsync();
           
            var cars =  query
                .Skip((page - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .Select(c => new CarAllViewModel
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Category = c.Category,
                    Hp = c.Hp,
                    ImageUrl = c.ImageUrl,
                    PricePerDay = c.PricePerDay,
                    IsRented = c.IsRented
                }).ToList();

            ViewBag.CurrentPage = page;

            return View(cars);
        }

        public async Task<IActionResult> Search(string? make, string? model, int? maxPrice)
        {
            var query = await carService.GetAllCarsAsync();
            query.AsQueryable();

            if (!string.IsNullOrEmpty(make))
            {
                query = query.Where(c => c.Make.ToLower()
                .Contains(make.ToLower()));
            }

            if (!string.IsNullOrEmpty(model))
            {
                query = query.Where(c => c.Model.ToLower()
                .Contains(model.ToLower()));
            }
            if (string.IsNullOrEmpty(model)&&string.IsNullOrEmpty(make)&&maxPrice is null)
            {
                return RedirectToAction("All", "Car");
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.PricePerDay <= maxPrice.Value);
            }

            return View(query);
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Rent(int carId)
        {
            var car = await carService.GetCarByIdAsync(carId);
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new RentBillInputModel();

            model.CarId = car.Id;
            model.UserId = userId;
            model.DateOfTaking = DateTime.UtcNow;

            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Rent(RentBillInputModel model)
        {
            return View();
        }
    }
}
