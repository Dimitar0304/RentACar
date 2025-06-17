using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.RentBillDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Core.Services
{
    public class RentBillService : IRentBillService
    {
        private readonly RentCarDbContext dbContext;

        public RentBillService(RentCarDbContext context)
        {
            dbContext = context;
        }

        public async Task<IEnumerable<RentBillViewModel>> GetAllRentBillsAsync()
        {
            var rentBills = await dbContext.Bills
                .Include(rb => rb.Car)
                .Include(rb => rb.User)
                .Select(rb => new RentBillViewModel
                {
                    Id = rb.Id,
                    CarId = rb.CarId,
                    CarMake = rb.Car.Make,
                    CarModel = rb.Car.Model,
                    UserId = rb.UserId,
                    UserFirstName = rb.User.FirstName,
                    UserLastName = rb.User.LastName,
                    DateOfTaking = rb.DateOfTaking,
                    DateOfReturn = rb.DateOfReturn,
                    TownOfRent = rb.TownOfRent,
                    StartMileage = rb.StartMileage,
                    EndMileage = rb.EndMileage,
                    TotalPrice = rb.TotalPrice
                })
                .ToListAsync();

            return rentBills;
        }

        public async Task<IEnumerable<RentBillViewModel>> GetUserRentBillsAsync(string userId)
        {
            var rentBills = await dbContext.Bills
                .Include(rb => rb.Car)
                .Include(rb => rb.User)
                .Where(rb => rb.UserId == userId)
                .Select(rb => new RentBillViewModel
                {
                    Id = rb.Id,
                    CarId = rb.CarId,
                    CarMake = rb.Car.Make,
                    CarModel = rb.Car.Model,
                    UserId = rb.UserId,
                    UserFirstName = rb.User.FirstName,
                    UserLastName = rb.User.LastName,
                    DateOfTaking = rb.DateOfTaking,
                    DateOfReturn = rb.DateOfReturn,
                    TownOfRent = rb.TownOfRent,
                    StartMileage = rb.StartMileage,
                    EndMileage = rb.EndMileage,
                    TotalPrice = rb.TotalPrice
                })
                .ToListAsync();

            return rentBills;
        }

        public async Task<RentBillViewModel?> GetRentBillByIdAsync(int rentBillId)
        {
            var rentBill = await dbContext.Bills
                .Include(rb => rb.Car)
                .Include(rb => rb.User)
                .FirstOrDefaultAsync(rb => rb.Id == rentBillId);

            if (rentBill == null)
            {
                return null;
            }

            return new RentBillViewModel
            {
                Id = rentBill.Id,
                CarId = rentBill.CarId,
                CarMake = rentBill.Car.Make,
                CarModel = rentBill.Car.Model,
                UserId = rentBill.UserId,
                UserFirstName = rentBill.User.FirstName,
                UserLastName = rentBill.User.LastName,
                DateOfTaking = rentBill.DateOfTaking,
                DateOfReturn = rentBill.DateOfReturn,
                TownOfRent = rentBill.TownOfRent,
                StartMileage = rentBill.StartMileage,
                EndMileage = rentBill.EndMileage,
                TotalPrice = rentBill.TotalPrice
            };
        }

        public async Task<bool> ReturnCarAsync(int rentBillId, int endMileage)
        {
            var rentBill = await dbContext.Bills
                .Include(rb => rb.Car)
                .FirstOrDefaultAsync(rb => rb.Id == rentBillId);

            if (rentBill == null || rentBill.DateOfReturn.HasValue)
            {
                return false;
            }

            rentBill.DateOfReturn = DateTime.UtcNow;
            rentBill.EndMileage = endMileage;
            rentBill.Car.Mileage = endMileage;
            rentBill.Car.IsRented = false;

            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateRentBillAsync(RentBillInputModel model)
        {
            var car = await dbContext.Cars.FindAsync(model.CarId);
            if (car == null || car.IsRented)
            {
                throw new InvalidOperationException("Car not available for rent");
            }

            var user = await dbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var rentBill = new RentBill
            {
                CarId = model.CarId,
                UserId = model.UserId,
                DateOfTaking = DateTime.UtcNow,
                DateOfReturn = model.DateOfReturn,
                TownOfRent = model.TownOfRent,
                StartMileage = car.Mileage,
                TotalPrice = model.DateOfTaking.Day - model.DateOfReturn.Value.Day * car.PricePerDay
            };

            car.IsRented = true;

            dbContext.Bills.Add(rentBill);
            await dbContext.SaveChangesAsync();

            return rentBill.Id;
        }
    }
} 