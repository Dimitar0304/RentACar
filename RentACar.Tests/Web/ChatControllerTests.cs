using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RentACar.Core.Models.ChatDto;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Account;
using RentACar.Web.Controllers;
using Xunit;
using FluentAssertions;
using RentACar.Tests.Helpers;
using RentACar.Tests.Constants;

namespace RentACar.Tests.Web
{
    public class ChatControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RentCarDbContext> _contextMock;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var userLoggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                optionsMock.Object,
                passwordHasherMock.Object,
                userValidators,
                passwordValidators,
                keyNormalizerMock.Object,
                errorsMock.Object,
                servicesMock.Object,
                userLoggerMock.Object);

            _contextMock = new Mock<RentCarDbContext>();
            _controller = new ChatController(_userManagerMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task Index_WhenUserNotAuthenticated_RedirectsToLogin()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Login");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Accounts");
        }

        [Fact]
        public async Task Index_WhenUserAuthenticated_ReturnsViewWithMessages()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = TestConstants.User.TestEmail };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var messages = new List<Message>
            {
                new Message { Id = 1, Content = "Test message 1", SenderId = "1", ReceiverId = "2", Timestamp = DateTime.UtcNow },
                new Message { Id = 2, Content = "Test message 2", SenderId = "2", ReceiverId = "1", Timestamp = DateTime.UtcNow }
            };

            _contextMock.Setup(x => x.Messages).Returns(DbSetMock.Create(messages.AsQueryable()).Object);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<MessageViewModel>;
            model.Should().NotBeNull();
            model.Should().HaveCount(2);
        }

        [Fact]
        public async Task SendMessage_WithValidData_SavesMessage()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", UserName = TestConstants.User.TestEmail };
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var messageViewModel = new MessageViewModel
            {
                Content = "Test message",
                ReceiverId = "2"
            };

            var messagesList = new List<Message>();
            var messagesDbSetMock = DbSetMock.Create(messagesList.AsQueryable());
            _contextMock.Setup(x => x.Messages).Returns(messagesDbSetMock.Object);

            _contextMock.Setup(x => x.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Callback<Message, CancellationToken>((msg, token) => messagesList.Add(msg))
                .ReturnsAsync((Message msg, CancellationToken token) => EntityEntryMock.Create(msg));

            _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.SendMessage(messageViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            messagesList.Should().ContainSingle(m => m.Content == "Test message");
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
} 