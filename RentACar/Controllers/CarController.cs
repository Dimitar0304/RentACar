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
        private readonly ICarService service;
        private readonly IRentBillService _rentBillService;
        private readonly RentCarDbContext _context;
        
        public CarController(ICarService carService, RentCarDbContext context, IRentBillService rentBillService)
        {
            service = carService;
            _context = context;
            _rentBillService = rentBillService;
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
                    PricePerDay = c.PricePerDay,
                    IsRented = c.IsRented
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
            if (string.IsNullOrEmpty(model)&&string.IsNullOrEmpty(make)&&maxPrice is null)
            {
                return RedirectToAction("All", "Car");
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

        [HttpGet]
        public async Task<IActionResult> Rent(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You must be logged in to rent a car.";
                return RedirectToAction(nameof(All));
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction(nameof(All));
            }

            if (car.IsRented)
            {
                TempData["ErrorMessage"] = "Car is already rented.";
                return RedirectToAction(nameof(All));
            }

            // Pre-populate the model with CarId and UserId
            var model = new RentBillInputModel
            {
                CarId = id,
                UserId = userId // UserId is set here, so [Required] is not needed on model
            };

            return View(model); // Returns the new Rent.cshtml view
        }

        [HttpPost]
        public async Task<IActionResult> Rent(RentBillInputModel model)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User not authenticated.";
                return RedirectToAction(nameof(All));
            }

            model.UserId = userId;

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    TempData["ErrorMessage"] += $"Error: {error.ErrorMessage} ";
                }
                if (string.IsNullOrEmpty(TempData["ErrorMessage"] as string))
                {
                     TempData["ErrorMessage"] = "Invalid rental dates or town provided.";
                }
                return View(model);
            }

            var car = await _context.Cars.FindAsync(model.CarId);

            if (car == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction(nameof(All));
            }

            if (car.IsRented)
            {
                TempData["ErrorMessage"] = "Car is already rented.";
                return RedirectToAction(nameof(All));
            }

            // Server-side date validation to prevent invalid date ranges
            if (model.DateOfTaking < DateTime.Today || model.DateOfReturn <= model.DateOfTaking)
            {
                ModelState.AddModelError(string.Empty, "Invalid rental date range. Rent date cannot be in the past, and return date must be after rent date.");
                return View(model); // Return the view to show date validation error
            }

            // Calculate TotalPrice and set StartMileage before sending to service
            TimeSpan rentalDuration = (model.DateOfReturn.Value - model.DateOfTaking);
            decimal numberOfDays = (decimal)Math.Ceiling(rentalDuration.TotalDays); 
            if (numberOfDays < 1) numberOfDays = 1; // Minimum 1 day rental

            model.TotalPrice = numberOfDays * car.PricePerDay;
            model.StartMileage = car.Mileage;

            try
            {
                await _rentBillService.CreateRentBillAsync(model);
                TempData["SuccessMessage"] = "Car rented successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred during the rental process.";
            }

            return RedirectToAction(nameof(All));
        }
    }
}
