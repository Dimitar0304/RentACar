using Xunit;
using Moq;
using FluentAssertions;
using RentACar.Core.Controllers;
using RentACar.Core.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task GetAll_ShouldReturnOkResult()
        {
            // Arrange
            var cars = new List<CarDto>();
            _carServiceMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(cars);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeEquivalentTo(cars);
        }
    }
} 