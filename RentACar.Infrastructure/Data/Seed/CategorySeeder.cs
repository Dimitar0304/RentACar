
using RentACar.Infrastructure.Data.Models.Vehicle;

namespace RentACar.Infrastructure.Data.Seed
{
    public class CategorySeeder : ISeeder
    {
        private readonly RentCarDbContext dbcontext;
        public CategorySeeder(RentCarDbContext context)
        {
            dbcontext = context;
        }
        public async Task SeedAsync()
        {
            dbcontext.AddRange(
                new Category { Id = 1, Name = "Mini" },
                new Category { Id = 2, Name = "Sedan" },
                new Category { Id = 3, Name = "Combi" },
                new Category { Id = 4, Name = "Van" },
                new Category { Id = 5, Name = "Luxury" },
                new Category { Id = 6, Name = "SUV" }
                );
            await dbcontext.SaveChangesAsync();
        }
    }
}
