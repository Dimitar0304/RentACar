using Microsoft.AspNet.Identity.EntityFramework;

namespace RentACar.Infrastructure.Data.Models
{
    class User:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public override string Email { get; set; } = null!;
        public int Role { get; set; }
        public string Phone { get; set; } = null!;
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

    }
}
