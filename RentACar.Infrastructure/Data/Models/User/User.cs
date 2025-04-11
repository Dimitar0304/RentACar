using Microsoft.AspNetCore.Identity;

namespace RentACar.Infrastructure.Data.Models.User
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public override string Email { get; set; } = null!;
        public int Role { get; set; }
        public string Phone { get; set; } = null!;
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        public ICollection<RentBill> RentBills { get; set; } = new List<RentBill>();

    }
}
