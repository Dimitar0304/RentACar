using RentACar.Infrastructure.Data.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Infrastructure.Data.Models.Vehicle
{
    public class Bus : IVehicle
    {
        public string Model { get; set; } = null!;
        public int Hp { get; set ; }
        public bool IsRented { get ; set ; }
        public int Mileage { get; set ; }
    }
}
