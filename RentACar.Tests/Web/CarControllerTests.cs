using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RentACar.Core.Models.CarDto;
using RentACar.Core.Services.Contracts;
using RentACar.Infrastructure.Data.Models.Vehicle;
using RentACar.Web.Controllers;
using Xunit;
using FluentAssertions;
using RentACar.Tests.Constants;

namespace RentACar.Tests.Web
{
    public class CarControllerTests
    {
        private readonly Mock<ICarService> _carServiceMock;
        private readonly CarController _controller;

        public CarControllerTests()
        {
            _carServiceMock = new Mock<ICarService>();
            _controller = new CarController(_carServiceMock.Object);
        }

        [Fact]
        public async Task All_ShouldReturnViewResult_WithListOfCarAllViewModel()
        {
            // Arrange
            var cars = new List<CarAllViewModel>
            {
                new CarAllViewModel
                {
                    Id = 1,
                    Make = TestConstants.Car.TestBrand,
                    Model = TestConstants.Car.TestModel,
                    Year = TestConstants.Car.TestYear,
                    PricePerDay = TestConstants.Car.TestPrice
                }
            };

            _carServiceMock.Setup(x => x.GetAllCarsAsync())
                .ReturnsAsync(cars);

            // Act
            var result = await _controller.All();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<CarAllViewModel>;
            model.Should().NotBeNull();
            model.Should().HaveCount(1);
            model.First().Make.Should().Be(TestConstants.Car.TestBrand);
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewResult_WithCarDetailsViewModel()
        {
            // Arrange
            var carId = 1;
            var car = new CarDetailsViewModel
            {
                Id = carId,
                Make = TestConstants.Car.TestBrand,
                Model = TestConstants.Car.TestModel,
                Year = TestConstants.Car.TestYear,
                PricePerDay = TestConstants.Car.TestPrice
            };

            _carServiceMock.Setup(x => x.GetCarByIdAsync(carId))
                .ReturnsAsync(car);

            // Act
            var result = await _controller.Details(carId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as CarDetailsViewModel;
            model.Should().NotBeNull();
            model.Id.Should().Be(carId);
            model.Make.Should().Be(TestConstants.Car.TestBrand);
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var carId = 999;
            _carServiceMock.Setup(x => x.GetCarByIdAsync(carId))
                .ReturnsAsync((CarDetailsViewModel)null);

            // Act
            var result = await _controller.Details(carId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Get_ShouldReturnViewResult()
        {
            // Act
            var result = await _controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Create_Post_WithValidData_ShouldRedirectToAll()
        {
            // Arrange
            var carViewModel = new CarViewModel
            {
                Make = TestConstants.Car.TestBrand,
                Model = TestConstants.Car.TestModel,
                Year = TestConstants.Car.TestYear,
                PricePerDay = TestConstants.Car.TestPrice
            };

            _carServiceMock.Setup(x => x.AddCarAsync(carViewModel))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(carViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("All");
            _carServiceMock.Verify(x => x.AddCarAsync(carViewModel), Times.Once);
        }

        [Fact]
        public async Task Create_Post_WithInvalidData_ShouldReturnViewResult()
        {
            // Arrange
            var carViewModel = new CarViewModel();
            _controller.ModelState.AddModelError("Make", "The Make field is required.");

            // Act
            var result = await _controller.Create(carViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ShouldReturnViewResult_WithCarViewModel()
        {
            // Arrange
            var carId = 1;
            var car = new CarViewModel
            {
                Id = carId,
                Make = TestConstants.Car.TestBrand,
                Model = TestConstants.Car.TestModel,
                Year = TestConstants.Car.TestYear,
                PricePerDay = TestConstants.Car.TestPrice
            };

            _carServiceMock.Setup(x => x.GetCarByIdAsync(carId))
                .ReturnsAsync(car);

            // Act
            var result = await _controller.Edit(carId);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as CarViewModel;
            model.Should().NotBeNull();
            model.Id.Should().Be(carId);
            model.Make.Should().Be(TestConstants.Car.TestBrand);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var carId = 999;
            _carServiceMock.Setup(x => x.GetCarByIdAsync(carId))
                .ReturnsAsync((CarViewModel)null);

            // Act
            var result = await _controller.Edit(carId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_Post_WithValidData_ShouldRedirectToAll()
        {
            // Arrange
            var carViewModel = new CarViewModel
            {
                Id = 1,
                Make = TestConstants.Car.TestBrand,
                Model = TestConstants.Car.TestModel,
                Year = TestConstants.Car.TestYear,
                PricePerDay = TestConstants.Car.TestPrice
            };

            _carServiceMock.Setup(x => x.UpdateCarAsync(carViewModel))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(carViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("All");
            _carServiceMock.Verify(x => x.UpdateCarAsync(carViewModel), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_WithInvalidData_ShouldReturnViewResult()
        {
            // Arrange
            var carViewModel = new CarViewModel();
            _controller.ModelState.AddModelError("Make", "The Make field is required.");

            // Act
            var result = await _controller.Edit(carViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldRedirectToAll()
        {
            // Arrange
            var carId = 1;
            _carServiceMock.Setup(x => x.DeleteCarAsync(carId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(carId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("All");
            _carServiceMock.Verify(x => x.DeleteCarAsync(carId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var carId = 999;
            _carServiceMock.Setup(x => x.DeleteCarAsync(carId))
                .ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller.Delete(carId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
} 