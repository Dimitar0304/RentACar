using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;

namespace RentACar.Controllers.CarController
{
    public class CarController:Controller
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
                return RedirectToAction("Home");
            }
        }
    }
}
