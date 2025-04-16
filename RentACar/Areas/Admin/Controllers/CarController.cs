using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;

namespace RentACar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
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
        public async Task<IActionResult> AddCar()
        {
            var model = new CarViewModel();
            model.Categories = await service.GetAllCategories();
            return View(model);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddCar(CarViewModel car)
        {
            if (!ModelState.IsValid)
            {
                car = new CarViewModel();
                car.Categories = await service.GetAllCategories();
                return View(car);
            }

            if (service.IsCarExistInDb(car))
            {
                ModelState.AddModelError("Error", "Car already exists");
                car.Categories = await service.GetAllCategories();
                return View(car);
            }

            await service.AddCarAsync(car);
            return RedirectToAction("All", "Car", new { area = "" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (await service.GetCarByIdAsync(id) == null)
            {
                return NotFound();
            }

            CarViewModel car = await service.GetCarByIdAsync(id);
            car.Categories = await service.GetAllCategories();

            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CarViewModel car)
        {
            if (car == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                car.Categories = await service.GetAllCategories();
                return View(car);
            }

            await service.UpdateCarAsync(car);
            return RedirectToAction("All", "Car", new { area = "" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0 || service.GetCarByIdAsync(id) == null)
            {
                return NotFound();
            }

            await service.DeleteCarAsync(id);
            return RedirectToAction("All", "Car", new { area = "" });
        }
    }
} 