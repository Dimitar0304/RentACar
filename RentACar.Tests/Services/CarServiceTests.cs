using Xunit;
using Moq;
using FluentAssertions;
using RentACar.Core.Services;
using RentACar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.Car;
using RentACar.Infrastructure.Data.Models.Car;

namespace RentACar.Tests.Services
{
    public class CarServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly ICarService _carService;

        public CarServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _contextMock = new Mock<ApplicationDbContext>(options);
            _carService = new CarService(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoCars_ReturnsEmptyList()
        {
            // Arrange
            var cars = new List<Car>();
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Provider).Returns(cars.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Expression).Returns(cars.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.ElementType).Returns(cars.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.GetEnumerator()).Returns(cars.GetEnumerator());

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            var result = await _carService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_WithCars_ReturnsAllCars()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Id = 1, Brand = "TestBrand1", Model = "TestModel1" },
                new Car { Id = 2, Brand = "TestBrand2", Model = "TestModel2" }
            };

            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Provider).Returns(cars.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Expression).Returns(cars.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.ElementType).Returns(cars.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.GetEnumerator()).Returns(cars.GetEnumerator());

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            var result = await _carService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCar()
        {
            // Arrange
            var car = new Car { Id = 1, Brand = "TestBrand", Model = "TestModel" };
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync(car);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            var result = await _carService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(999)).ReturnsAsync((Car)null);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            var result = await _carService.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddAsync_WithValidData_AddsCar()
        {
            // Arrange
            var carDto = new CarDto
            {
                Brand = TestConstants.Car.TestBrand,
                Model = TestConstants.Car.TestModel,
                Year = TestConstants.Car.TestYear,
                Price = TestConstants.Car.TestPrice
            };

            var dbSetMock = new Mock<DbSet<Car>>();
            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            await _carService.AddAsync(carDto);

            // Assert
            dbSetMock.Verify(x => x.Add(It.IsAny<Car>()), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WithExistingCar_ThrowsException()
        {
            // Arrange
            var existingCar = new Car { Id = 1, Brand = "TestBrand", Model = "TestModel" };
            var carDto = new CarDto { Brand = "TestBrand", Model = "TestModel" };

            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Provider)
                .Returns(new List<Car> { existingCar }.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.Expression)
                .Returns(new List<Car> { existingCar }.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.ElementType)
                .Returns(new List<Car> { existingCar }.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Car>>().Setup(m => m.GetEnumerator())
                .Returns(new List<Car> { existingCar }.GetEnumerator());

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _carService.AddAsync(carDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_DeletesCar()
        {
            // Arrange
            var car = new Car { Id = 1, Brand = "TestBrand", Model = "TestModel" };
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync(car);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            await _carService.DeleteAsync(1);

            // Assert
            dbSetMock.Verify(x => x.Remove(car), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ThrowsException()
        {
            // Arrange
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(999)).ReturnsAsync((Car)null);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _carService.DeleteAsync(999));
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesCar()
        {
            // Arrange
            var existingCar = new Car { Id = 1, Brand = "OldBrand", Model = "OldModel" };
            var carDto = new CarDto { Id = 1, Brand = "NewBrand", Model = "NewModel" };

            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(1)).ReturnsAsync(existingCar);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act
            await _carService.UpdateAsync(carDto);

            // Assert
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentCar_ThrowsException()
        {
            // Arrange
            var carDto = new CarDto { Id = 999, Brand = "NewBrand", Model = "NewModel" };
            var dbSetMock = new Mock<DbSet<Car>>();
            dbSetMock.Setup(x => x.FindAsync(999)).ReturnsAsync((Car)null);

            _contextMock.Setup(x => x.Cars).Returns(dbSetMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _carService.UpdateAsync(carDto));
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };

            var dbSetMock = new Mock<DbSet<Category>>();
            dbSetMock.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.AsQueryable().Provider);
            dbSetMock.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            dbSetMock.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            dbSetMock.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.GetEnumerator());

            _contextMock.Setup(x => x.Categories).Returns(dbSetMock.Object);

            // Act
            var result = await _carService.GetAllCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
} 