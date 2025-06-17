using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Models.RentBillDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using System.Security.Claims;

namespace RentACar.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarService carService;
        private readonly IRentBillService rentBillService;
        
        public CarController(ICarService _carService,  IRentBillService _rentBillService)
        {
            carService = _carService;
            rentBillService = _rentBillService;
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
            var car = await carService.GetCarByIdAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
        [HttpPost]
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
        public async Task<IActionResult> CreateARent(RentBillInputModel model)
        {
           await rentBillService.CreateRentBillAsync(model);

          return RedirectToAction("All", "Car");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewInbox()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var models = await rentBillService.GetUserRentBillsAsync(userId);

            return View(models);
        }

    }
}
