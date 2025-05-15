using Xunit;
using Moq;
using FluentAssertions;
using RentACar.Core.Services.CarDto;
using RentACar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using RentACar.Core.Models.CarDto;
using RentACar.Infrastructure.Data.Models.Vehicle;
using RentACar.Core.Services.Contracts;
using RentACar.Core.Models.CategoryDto;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Threading;

namespace RentACar.Tests.Services
{
    public class CarServiceTests
    {
        private readonly Mock<RentCarDbContext> _contextMock;
        private readonly ICarService _carService;

        public CarServiceTests()
        {
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _contextMock = new Mock<RentCarDbContext>(options);
            _carService = new CarService(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllCarsAsync_WhenNoCars_ReturnsEmptyList()
        {
            // Arrange
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car>().AsQueryable()).Object);

            // Act
            var result = await _carService.GetAllCarsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCarsAsync_WithCars_ReturnsAllCars()
        {
            // Arrange
            var categoryForTest = new Category { Id = 1, Name = "TestCategory" };
            var carsInDb = new List<Car>
            {
                new Car { Id = 1, Make = "TestBrand1", Model = "TestModel1", Category = categoryForTest, CategoryId = 1, Hp = 100, ImageUrl = "test1.jpg", Mileage = 1000, PricePerDay = 50, IsRented = false },
                new Car { Id = 2, Make = "TestBrand2", Model = "TestModel2", Category = categoryForTest, CategoryId = 1, Hp = 120, ImageUrl = "test2.jpg", Mileage = 2000, PricePerDay = 60, IsRented = false }
            };
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(carsInDb.AsQueryable()).Object);

            // Act
            var result = await _carService.GetAllCarsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Make.Should().Be("TestBrand1");
        }

        [Fact]
        public async Task GetCarByIdAsync_WithValidId_ReturnsCarViewModel()
        {
            // Arrange
            var categoryForTest = new Category { Id = 1, Name = "TestCategory" };
            var carEntity = new Car { Id = 1, Make = "TestBrand", Model = "TestModel", CategoryId = 1, Category = categoryForTest, Hp = 100, ImageUrl = "test.jpg", Mileage = 1000, PricePerDay = 50, IsRented = false };
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car> { carEntity }.AsQueryable()).Object);

            // Act
            var result = await _carService.GetCarByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Make.Should().Be("TestBrand");
        }

        [Fact]
        public async Task GetCarByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car>().AsQueryable()).Object);

            // Act
            var result = await _carService.GetCarByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddCarAsync_WithValidData_AddsCar()
        {
            // Arrange
            var carViewModel = new CarViewModel
            {
                Make = "TestBrandAdd",
                Model = "TestModelAdd",
                PricePerDay = 100,
                CategoryId = 1,
                Hp = 150,
                ImageUrl = "testadd.jpg",
                Mileage = 0
            };

            var testCategory = new Category { Id = 1, Name = "TestCategory" };
            _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(new List<Category> { testCategory }.AsQueryable()).Object);
            
            var carsList = new List<Car>();
            // No longer need carsDbSetMock for AddAsync verification directly, but Cars DbSet still needs to be generally available if service reads from it.
            // For this specific test, _context.Cars is not read before AddAsync, so a basic mock is fine.
            var carsDbSetMock = DbSetMock.Create(carsList.AsQueryable()); 
            _contextMock.Setup(x => x.Cars).Returns(carsDbSetMock.Object); // Keep this for general DbSet availability

            // Setup and verify DbContext.AddAsync directly
            _contextMock.Setup(x => x.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
               .Callback<Car, CancellationToken>((car, token) => carsList.Add(car))
               .ReturnsAsync((Car car, CancellationToken token) => EntityEntryMock.Create(car));

            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _carService.AddCarAsync(carViewModel);

            // Assert
            _contextMock.Verify(x => x.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Once); // Verify context.AddAsync
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            carsList.Should().ContainSingle(c => c.Make == "TestBrandAdd");
        }
        
        [Fact]
        public async Task AddCarAsync_WithNonExistentCategory_ThrowsArgumentException()
        {
            // Arrange
            var carViewModel = new CarViewModel
            {
                Make = "TestBrandAdd",
                Model = "TestModelAdd",
                PricePerDay = 100,
                CategoryId = 999,
                Hp = 150,
                ImageUrl = "testadd.jpg",
                Mileage = 0
            };

            _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(new List<Category>().AsQueryable()).Object);
             var carsDbSetMock = DbSetMock.Create(new List<Car>().AsQueryable());
            _contextMock.Setup(x => x.Cars).Returns(carsDbSetMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _carService.AddCarAsync(carViewModel));
        }

        [Fact]
        public async Task IsCarExistInDb_WhenCarExists_ReturnsTrue()
        {
            // Arrange
            var testCategory = new Category { Id = 1, Name = "TestCategory" };
            var existingCar = new Car { Id = 1, Make = "TestBrand", Model = "TestModel", CategoryId = 1, Category = testCategory, Hp = 100, ImageUrl = "test.jpg", Mileage = 100, PricePerDay = 50, IsRented = false };
            var carViewModel = new CarViewModel { Id = 1, Make = "TestBrand", Model = "TestModel", CategoryId = 1 };
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car> { existingCar }.AsQueryable()).Object);

            // Act
            var result = _carService.IsCarExistInDb(carViewModel);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsCarExistInDb_WhenCarDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var carViewModel = new CarViewModel { Id = 2, Make = "NewBrand", Model = "NewModel", CategoryId = 1 };
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car>().AsQueryable()).Object);
        
            // Act
            var result = _carService.IsCarExistInDb(carViewModel);
        
            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteCarAsync_WithValidId_DeletesCar()
        {
            // Arrange
            var testCategory = new Category { Id = 1, Name = "TestCategory" };
            var carToDelete = new Car { Id = 1, Make = "TestBrand", Model = "TestModel", Category = testCategory, CategoryId = 1, Hp = 100, ImageUrl = "test.jpg", Mileage = 100, PricePerDay = 50, IsRented = false };
            var carsList = new List<Car> { carToDelete };
            var carsDbSetMock = DbSetMock.Create(carsList.AsQueryable());
             carsDbSetMock.Setup(m => m.Remove(It.IsAny<Car>()))
                .Callback<Car>(c => carsList.Remove(c));

            _contextMock.Setup(x => x.Cars).Returns(carsDbSetMock.Object);
            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _carService.DeleteCarAsync(1);

            // Assert
            carsDbSetMock.Verify(x => x.Remove(It.Is<Car>(c => c.Id == 1)), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            carsList.Should().NotContain(c => c.Id == 1);
        }

        [Fact]
        public async Task DeleteCarAsync_WithInvalidId_DoesNotDeleteAndDoesNotSaveChanges()
        {
            // Arrange
            var carsDbSetMock = DbSetMock.Create(new List<Car>().AsQueryable());
            _contextMock.Setup(x => x.Cars).Returns(carsDbSetMock.Object);

            // Act
            await _carService.DeleteCarAsync(999);

            // Assert
            carsDbSetMock.Verify(x => x.Remove(It.IsAny<Car>()), Times.Never);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCarAsync_WithValidData_UpdatesCar()
        {
            // Arrange
            var categoryForTest = new Category { Id = 1, Name = "TestCategory" };
            var existingCarEntity = new Car { Id = 1, Make = "OldBrand", Model = "OldModel", CategoryId = 1, Category = categoryForTest, Hp = 100, ImageUrl = "old.jpg", Mileage = 1000, PricePerDay = 50, IsRented = false };
            var carViewModel = new CarViewModel { Id = 1, Make = "NewBrand", Model = "NewModel", CategoryId = 1, Hp = 150, ImageUrl = "new.jpg", Mileage = 1500, PricePerDay = 60 };

             _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(new List<Category> { categoryForTest }.AsQueryable()).Object);
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car> { existingCarEntity }.AsQueryable()).Object);
            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _carService.UpdateCarAsync(carViewModel);

            // Assert
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            existingCarEntity.Make.Should().Be("NewBrand");
            existingCarEntity.Model.Should().Be("NewModel");
            existingCarEntity.Hp.Should().Be(150);
        }

        [Fact]
        public async Task UpdateCarAsync_WithNonExistentCar_ThrowsArgumentException()
        {
            // Arrange
            var carViewModel = new CarViewModel { Id = 999, Make = "NewBrand", Model = "NewModel", CategoryId = 1 };
            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car>().AsQueryable()).Object);
            var categoryForTest = new Category { Id = 1, Name = "TestCategory" };
            _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(new List<Category> { categoryForTest }.AsQueryable()).Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _carService.UpdateCarAsync(carViewModel));
        }
        
        [Fact]
        public async Task UpdateCarAsync_WithNonExistentCategory_ThrowsArgumentException()
        {
            // Arrange
            var categoryForTest = new Category { Id = 1, Name = "TestCategory" }; // This category will be used for existingCarEntity
            var existingCarEntity = new Car { Id = 1, Make = "OldBrand", Model = "OldModel", CategoryId = 1, Category = categoryForTest, Hp = 100, ImageUrl = "old.jpg", Mileage = 1000, PricePerDay = 50, IsRented = false};
            var carViewModel = new CarViewModel { Id = 1, Make = "NewBrand", Model = "NewModel", CategoryId = 999, Hp = 150, ImageUrl = "new.jpg", Mileage = 1500, PricePerDay = 60 };

            _contextMock.Setup(x => x.Cars).Returns(DbSetMock.Create(new List<Car> { existingCarEntity }.AsQueryable()).Object);
            _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(new List<Category>().AsQueryable()).Object); // No categories with ID 999

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _carService.UpdateCarAsync(carViewModel));
        }

        [Fact]
        public async Task GetAllCategories_ReturnsAllCategories()
        {
            // Arrange
            var categoriesInDb = new List<Category>
            {
                new Category { Id = 1, Name = "Category1" },
                new Category { Id = 2, Name = "Category2" }
            };
            _contextMock.Setup(x => x.Categories).Returns(DbSetMock.Create(categoriesInDb.AsQueryable()).Object);

            // Act
            var result = await _carService.GetAllCategories();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Category1");
        }
    }

    public static class DbSetMock
    {
        public static Mock<DbSet<T>> Create<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            // Setup IQueryable properties
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryableData.Provider));
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            // GetEnumerator should return the enumerator of the TestAsyncEnumerable which wraps the original data
            // Cast to IEnumerable<T> to access the explicit interface implementation of GetEnumerator
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<T>)new TestAsyncEnumerable<T>(queryableData)).GetEnumerator());

            // Setup IAsyncEnumerable
            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryableData.GetEnumerator()));

            // Setup AddAsync
            if (data is ICollection<T> collection)
            {
                 dbSetMock.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                    .Callback<T, CancellationToken>((entity, token) => collection.Add(entity))
                    .ReturnsAsync((T entity, CancellationToken token) => EntityEntryMock.Create(entity));
            }
            else // Fallback if data is not ICollection for some reason (should not happen with List)
            {
                 dbSetMock.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((T entity, CancellationToken token) => EntityEntryMock.Create(entity));
            }


            dbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => {
                    if (ids == null || ids.Length == 0) return null;
                    var idProperty = typeof(T).GetProperty("Id");
                    if (idProperty == null) return null;

                    var idToFind = ids[0];
                    try
                    {
                        var convertedId = Convert.ChangeType(idToFind, idProperty.PropertyType);
                         return data.FirstOrDefault(GetPredicate<T>(new object[] { convertedId }).Compile());
                    }
                    catch (InvalidCastException)
                    {
                        return null;
                    }
                });
            
            if (data is ICollection<T> listForRemove)
            {
                dbSetMock.Setup(m => m.Remove(It.IsAny<T>()))
                    .Callback<T>((entity) => listForRemove.Remove(entity));
            }

            return dbSetMock;
        }

        private static Expression<Func<T, bool>> GetPredicate<T>(object[] ids) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var idPropertyInfo = typeof(T).GetProperty("Id");
            if (idPropertyInfo == null) 
                throw new InvalidOperationException($"Type {typeof(T).Name} does not have an 'Id' property for FindAsync mocking.");

            var idValue = Convert.ChangeType(ids[0], idPropertyInfo.PropertyType);
            var body = Expression.Equal(Expression.Property(parameter, idPropertyInfo), Expression.Constant(idValue));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }

    // region Async Test Helpers from CarControllerTests.cs
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            if (expression is MethodCallExpression methodCall && 
                methodCall.Method.Name == "FirstOrDefaultAsync" &&
                methodCall.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions))
            {
                Expression sourceExpression = methodCall.Arguments[0];
                // TResult is the element type expected from FirstOrDefaultAsync.
                // We create a queryable for the source part of the expression.
                // The CreateQuery<TElement> method on IQueryProvider creates an IQueryable<TElement>.
                // Here, TElement should be TResult.
                IQueryable<TResult> queryableSource = this.CreateQuery<TResult>(sourceExpression);

                if (methodCall.Arguments.Count == 2) // FirstOrDefaultAsync(source, predicate)
                {
                    LambdaExpression predicateLambda = StripQuotes(methodCall.Arguments[1]) as LambdaExpression;
                    // Call Queryable.FirstOrDefault<TResult>(source, predicate)
                    return queryableSource.Provider.Execute<TResult>(
                        Expression.Call(
                            typeof(Queryable), "FirstOrDefault",
                            new Type[] { queryableSource.ElementType },
                            queryableSource.Expression, predicateLambda));
                }
                else // FirstOrDefaultAsync(source)
                {
                    // Call Queryable.FirstOrDefault<TResult>(source)
                     return queryableSource.Provider.Execute<TResult>(
                        Expression.Call(
                            typeof(Queryable), "FirstOrDefault",
                            new Type[] { queryableSource.ElementType },
                            queryableSource.Expression));
                }
            }

            // Fallback for other async methods (like ToListAsync, which seems to work with this Execute call)
            // or non-async expressions.
            return this.Execute<TResult>(expression); 
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            // Important: this must return an IAsyncEnumerator that works with the test setup.
            // The base.AsEnumerable().GetEnumerator() gives a standard IEnumerator<T>.
            // TestAsyncEnumerator wraps this to provide the async interface.
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        // Crucially, this QueryProvider should be the TestAsyncQueryProvider that knows how to handle async.
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this); // 'this' is an IQueryable
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
    // endregion

    // Minimal mock for EntityEntry to satisfy AddAsync. 
    // If more complex interactions with EntityEntry are needed, this would need expansion.
    internal static class EntityEntryMock
    {
        public static Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Create<TEntity>(TEntity entity) where TEntity : class
        {
            // EntityEntry<T> has internal constructors, Moq can't proxy it easily with new Mock<>()
            // If the code under test doesn't interact with the returned EntityEntry, returning null is often simplest.
            return null; // Or default(EntityEntry<TEntity>)
        }
         public static Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Create(object entity)
        {
            return null; // Or default(EntityEntry)
        }
    }
}