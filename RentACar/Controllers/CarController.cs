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
        public async Task<IActionResult> Delete(int id)
        {
            if (id==0||service.GetCarByIdAsync(id)==null)
            {
                return BadRequest();
            }
            await service.DeleteCarAsync(id);

            return RedirectToAction("Index", "Home");
        }
    }
}
