namespace RentACar.Infrastructure.Data.Models.Interfaces
{
     public interface IVehicle
    {
        public string Model { get; set; }
        public int Hp { get; set; }
        public bool IsRented { get; set; }
        public int Mileage { get; set; }
    }
}
