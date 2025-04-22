public interface IRentBillService
    {
        Task StartRental(int carId, string userId);
        Task EndRental(int carId, int endMileage);
        // ... other existing methods ...
    }