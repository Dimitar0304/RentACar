using Microsoft.AspNetCore.Identity;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Infrastructure.Data.Seed
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@rentacar.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    Phone = "0000000000",
                    DateRegistered = DateTime.UtcNow,
                    Role = 1 // Assuming 1 represents Administrator role
                };

                await _userManager.CreateAsync(adminUser, "Admin123!");
                await _userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
} 