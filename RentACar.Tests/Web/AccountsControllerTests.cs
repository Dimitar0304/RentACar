using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RentACar.Core.Models.AccountDto;
using RentACar.Infrastructure.Data;
using RentACar.Infrastructure.Data.Models.Account;
using RentACar.Web.Controllers;
using Xunit;
using FluentAssertions;
using RentACar.Tests.Helpers;

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
        private readonly Mock<RentCarDbContext> _contextMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
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

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signInLoggerMock = new Mock<ILogger<SignInManager<ApplicationUser>>>();

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                optionsMock.Object,
                signInLoggerMock.Object,
                servicesMock.Object);

            _contextMock = new Mock<RentCarDbContext>();
            _controller = new AccountsController(_userManagerMock.Object, _signInManagerMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task Login_Get_WhenUserNotAuthenticated_ReturnsView()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(false);

            // Act
            var result = await _controller.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_Get_WhenUserAuthenticated_RedirectsToHome()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(true);

            // Act
            var result = await _controller.Login();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_Post_WithValidCredentials_RedirectsToHome()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            var user = new ApplicationUser { Email = loginViewModel.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false))
                .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _controller.Login(loginViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewWithModelError()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "WrongPassword123!",
                RememberMe = true
            };

            var user = new ApplicationUser { Email = loginViewModel.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(loginViewModel.Email))
                .ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _controller.Login(loginViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
            _controller.ModelState[""].Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("Invalid login attempt.");
        }

        [Fact]
        public async Task Register_Get_WhenUserNotAuthenticated_ReturnsView()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(false);

            // Act
            var result = await _controller.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Register_Get_WhenUserAuthenticated_RedirectsToHome()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.IsSignedIn(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns(true);

            // Act
            var result = await _controller.Register();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Home");
        }

        [Fact]
        public async Task Register_Post_WithValidData_CreatesUserAndRedirectsToHome()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerViewModel.Password))
                .ReturnsAsync(IdentityResult.Success);

            _signInManagerMock.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), false, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Register(registerViewModel);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Home");
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerViewModel.Password), Times.Once);
            _signInManagerMock.Verify(x => x.SignInAsync(It.IsAny<ApplicationUser>(), false, null), Times.Once);
        }

        [Fact]
        public async Task Register_Post_WithInvalidData_ReturnsViewWithModelError()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword123!",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _controller.Register(registerViewModel);

            // Assert
            result.Should().BeOfType<ViewResult>();
            _controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Logout_RedirectsToHome()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.SignOutAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be("Index");
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ControllerName.Should().Be("Home");
            _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
        }
    }
} 