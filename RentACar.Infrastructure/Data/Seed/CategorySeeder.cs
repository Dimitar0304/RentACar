
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
                new Category {  Name = "Mini" },
                new Category {  Name = "Sedan" },
                new Category {  Name = "Combi" },
                new Category { Name = "Van" },
                new Category { Name = "Luxury" },
                new Category { Name = "SUV" }
                );
            await dbcontext.SaveChangesAsync();
        }
    }
}
