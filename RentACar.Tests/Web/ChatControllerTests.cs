using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RentACar.Controllers;
using RentACar.Core.Services.Contracts;

namespace RentACar.Tests.Web
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> _chatServiceMock;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _chatServiceMock = new Mock<IChatService>();
            _controller = new ChatController(_chatServiceMock.Object);
        }

        [Fact]
        public void Index_WhenCalled_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
} 