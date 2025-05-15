using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RentACar.Controllers;
using RentACar.Core.Models.Account;
using RentACar.Infrastructure.Data.Models.User;

namespace RentACar.Tests.Web
{
    // Define TestConstants if not available elsewhere in the test project
    internal static class TestConstants 
    {
        internal static class User
        {
            internal const string TestEmail = "test@example.com";
            internal const string TestPassword = "Password123!";
            internal const string TestFirstName = "Test";
            internal const string TestLastName = "User";
        }
    }

    public class AccountsControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null, null, null, null);

            _controller = new AccountsController(_userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public void Login_Get_WhenUserNotAuthenticated_ReturnsView()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_Post_WithValidCredentials_RedirectsToHome()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = TestConstants.User.TestEmail,
                Password = TestConstants.User.TestPassword
            };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(
                model.Email,
                model.Password,
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _controller.Login(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Register_Post_WithValidData_CreatesUserAndRedirects()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = TestConstants.User.TestEmail,
                Password = TestConstants.User.TestPassword,
                FirstName = TestConstants.User.TestFirstName,
                LastName = TestConstants.User.TestLastName
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Logout_Post_RedirectsToHome()
        {
            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }
    }
} 