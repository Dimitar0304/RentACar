using Xunit;
using Moq;
using FluentAssertions;
using RentACar.Controllers;
using RentACar.Core.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using RentACar.Core.Models.CarDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using RentACar.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;

namespace RentACar.Tests.Web
{
    public class CarControllerTests
    {
        private readonly Mock<ICarService> _carServiceMock;
        private readonly Mock<RentCarDbContext> _dbContextMock;
        private readonly CarController _controller;

        public CarControllerTests()
        {
            _carServiceMock = new Mock<ICarService>();
            var options = new DbContextOptionsBuilder<RentCarDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _dbContextMock = new Mock<RentCarDbContext>(options);

            _controller = new CarController(_carServiceMock.Object, _dbContextMock.Object);
        }

        [Fact]
        public async Task All_ShouldReturnViewResult_WithListOfCarAllViewModel()
        {
            // Arrange
            var expectedCars = new List<CarAllViewModel>
            {
                new CarAllViewModel { Id = 1, Make = "Toyota", Model = "Camry", Hp = 180, ImageUrl = "url1", PricePerDay = 50, Category = "Sedan" },
                new CarAllViewModel { Id = 2, Make = "Honda", Model = "Civic", Hp = 150, ImageUrl = "url2", PricePerDay = 40, Category = "Sedan" }
            };
            
            _carServiceMock.Setup(s => s.GetAllCarsAsync()).ReturnsAsync(expectedCars);

            var carEntities = new List<RentACar.Infrastructure.Data.Models.Vehicle.Car>
            {
                 new RentACar.Infrastructure.Data.Models.Vehicle.Car { Id = 1, Make = "Toyota", Model = "Camry", CategoryId = 1, Hp = 180, ImageUrl = "url1", PricePerDay = 50, Category = new RentACar.Infrastructure.Data.Models.Vehicle.Category{Id = 1, Name="Sedan"} },
                 new RentACar.Infrastructure.Data.Models.Vehicle.Car { Id = 2, Make = "Honda", Model = "Civic", CategoryId = 1, Hp = 150, ImageUrl = "url2", PricePerDay = 40, Category = new RentACar.Infrastructure.Data.Models.Vehicle.Category{Id = 1, Name="Sedan"} }
            }.AsQueryable();

            var dbSetMock = new Mock<DbSet<RentACar.Infrastructure.Data.Models.Vehicle.Car>>();
            dbSetMock.As<IQueryable<RentACar.Infrastructure.Data.Models.Vehicle.Car>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<RentACar.Infrastructure.Data.Models.Vehicle.Car>(carEntities.Provider));
            dbSetMock.As<IQueryable<RentACar.Infrastructure.Data.Models.Vehicle.Car>>().Setup(m => m.Expression).Returns(carEntities.Expression);
            dbSetMock.As<IQueryable<RentACar.Infrastructure.Data.Models.Vehicle.Car>>().Setup(m => m.ElementType).Returns(carEntities.ElementType);
            dbSetMock.As<IQueryable<RentACar.Infrastructure.Data.Models.Vehicle.Car>>().Setup(m => m.GetEnumerator()).Returns(() => ((IEnumerable<RentACar.Infrastructure.Data.Models.Vehicle.Car>)carEntities).GetEnumerator());
            
            dbSetMock.As<IAsyncEnumerable<RentACar.Infrastructure.Data.Models.Vehicle.Car>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<RentACar.Infrastructure.Data.Models.Vehicle.Car>(((IEnumerable<RentACar.Infrastructure.Data.Models.Vehicle.Car>)carEntities).GetEnumerator()));

            _dbContextMock.Setup(c => c.Cars).Returns(dbSetMock.Object);

            // Act
            var result = await _controller.All();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<CarAllViewModel>>().Subject.ToList();
            model.Should().HaveCount(2);
            
            model[0].Category.Should().Be("Sedan");
            model[1].Category.Should().Be("Sedan");

            model.Should().BeEquivalentTo(expectedCars);
        }
    }

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
            return Execute<TResult>(expression);
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
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
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
} 