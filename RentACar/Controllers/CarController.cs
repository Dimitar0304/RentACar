using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;

namespace RentACar.Controllers
{

    public class CarController : Controller
    {
        private readonly ICarService service;
        public CarController(ICarService carService)
        {
            service = carService;
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
                ModelState.AddModelError("Error", "");
                car.Categories = await service.GetAllCategories();
                return View(car);
            }
            else
            {
                await service.AddCarAsync(car);
                return RedirectToAction("Index","Home");
            }
        }
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var models = await service.GetAllCarsAsync();
            if (models != null)
            {
                return View(models);
            }
            else
            {
                return RedirectToAction("Home");
            }

        }
        public async Task<IActionResult> Edit(int id)
        {
            if (await service.GetCarByIdAsync(id)==null)
            {
                return BadRequest();
            }
            CarViewModel car = await service.GetCarByIdAsync(id);
            car.Categories = await service.GetAllCategories();

            return View("Edit", car);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CarViewModel car)
        {
            if (car ==null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid==false)
            {
                car.Categories = await service.GetAllCategories();
                return View(car);
            }
            await service.UpdateCarAsync(car);
            return RedirectToAction("All", "Car");
        }
    }
}
