using FitnessTracker.DbContext;
using FitnessTracker.DTOs.Auth;
using FitnessTracker.DTOs.Refresh;
using FitnessTracker.Models;
using FitnessTracker.Services;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class AuthServiceTests
{
    // Each test gets its own uniquely-named InMemory database, so tests
    // never see each other's data even when run in parallel.
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    // UserManager<AppUser> has a lot of constructor dependencies we don't
    // care about in a unit test. Its public methods are all `virtual`
    // specifically so they can be mocked like this - this is the standard
    // pattern for testing anything that depends on ASP.NET Core Identity.
    private static Mock<UserManager<AppUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<AppUser>>();
        var manager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
        // Default: nobody has registered yet, so Register() treats the
        // next signup as the first user unless a test overrides this.
        manager.Setup(m => m.Users).Returns(new List<AppUser>().AsQueryable());
        return manager;
    }

    private static IConfiguration CreateConfiguration(string? bootstrapAdminEmail = null)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Admin:BootstrapEmail"] = bootstrapAdminEmail
            })
            .Build();
    }

    // AuthService.Login/RefreshToken return `object` (an anonymous type).
    // Anonymous types are `internal`, so a different assembly (this test
    // project) can't cast to them directly - reflection sidesteps that.
    private static string? GetPropertyValue(object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        Assert.NotNull(property);
        return property!.GetValue(obj) as string;
    }

    [Fact]
    public async Task Register_WithValidData_CreatesUserWithCorrectDetails()
    {
        // Arrange
        var userManager = CreateUserManagerMock();
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), "Password123!"))
            .ReturnsAsync(IdentityResult.Success);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());
        var dto = new RegisterDto("test@example.com", "Password123!");

        // Act
        await sut.Register(dto);

        // Assert
        userManager.Verify(
            m => m.CreateAsync(
                It.Is<AppUser>(u => u.Email == "test@example.com" && u.UserName == "test@example.com"),
                "Password123!"),
            Times.Once);
    }

    [Fact]
    public async Task Register_WhenIdentityRejectsTheUser_ThrowsInvalidOperationException()
    {
        var userManager = CreateUserManagerMock();
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Register(new RegisterDto("test@example.com", "weak")));

        Assert.Contains("Password too weak", ex.Message);
    }

    [Fact]
    public async Task Register_WhenFirstUserToRegister_IsAssignedAdminRole()
    {
        // There's no admin UI, so the only way anyone ever becomes an
        // Admin is by being the first person to ever sign up.
        var userManager = CreateUserManagerMock();
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await sut.Register(new RegisterDto("first@example.com", "Password123!"));

        userManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task Register_WhenNotTheFirstUser_IsNotAssignedAdminRole()
    {
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.Users).Returns(new List<AppUser>
        {
            new AppUser { Id = Guid.NewGuid(), Email = "existing@example.com", UserName = "existing@example.com" }
        }.AsQueryable());
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await sut.Register(new RegisterDto("second@example.com", "Password123!"));

        userManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Register_WhenEmailMatchesBootstrapAdmin_IsAssignedAdminRoleEvenIfNotFirstUser()
    {
        // Admin:BootstrapEmail lets a specific deployer-chosen address always
        // become Admin, regardless of registration order - the whole point
        // is to not depend on who happens to sign up first.
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.Users).Returns(new List<AppUser>
        {
            new AppUser { Id = Guid.NewGuid(), Email = "existing@example.com", UserName = "existing@example.com" }
        }.AsQueryable());
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), CreateConfiguration("admin@admin.com"));

        await sut.Register(new RegisterDto("admin@admin.com", "Password123!"));

        userManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task Register_WhenEmailDoesNotMatchBootstrapAdminAndIsNotFirstUser_IsNotAssignedAdminRole()
    {
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.Users).Returns(new List<AppUser>
        {
            new AppUser { Id = Guid.NewGuid(), Email = "existing@example.com", UserName = "existing@example.com" }
        }.AsQueryable());
        userManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), CreateConfiguration("admin@admin.com"));

        await sut.Register(new RegisterDto("someone-else@example.com", "Password123!"));

        userManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
    {
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => sut.Login(new LoginDto("ghost@example.com", "whatever")));
    }

    [Fact]
    public async Task Login_WhenPasswordIsWrong_ThrowsUnauthorizedAccessException()
    {
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "wrong")).ReturnsAsync(false);
        var sut = new AuthService(userManager.Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.Login(new LoginDto(user.Email!, "wrong")));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokensAndPersistsRefreshToken()
    {
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        var userManager = CreateUserManagerMock();
        userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "correct")).ReturnsAsync(true);

        userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string>());

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.CreateToken(user, It.IsAny<IEnumerable<string>>())).Returns("access-token");
        tokenService.Setup(t => t.GenerateRefreshToken()).Returns("refresh-token");

        var context = CreateContext();
        var sut = new AuthService(userManager.Object, tokenService.Object, context, Mock.Of<IConfiguration>());

        var result = await sut.Login(new LoginDto(user.Email!, "correct"));

        Assert.Equal("access-token", GetPropertyValue(result, "accessToken"));
        Assert.Equal("refresh-token", GetPropertyValue(result, "refreshToken"));

        var stored = await context.RefreshTokens.SingleAsync();
        Assert.Equal("refresh-token", stored.Token);
        Assert.Equal(user.Id, stored.UserId);
        Assert.False(stored.IsRevoked);
    }

    [Fact]
    public async Task RefreshToken_WhenTokenDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        var sut = new AuthService(CreateUserManagerMock().Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.RefreshToken(new TokenRefreshRequestDto { RefreshToken = "does-not-exist" }));
    }

    [Fact]
    public async Task RefreshToken_WhenTokenIsExpired_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        context.Users.Add(user);
        context.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            IsRevoked = false
        });
        await context.SaveChangesAsync();

        var sut = new AuthService(CreateUserManagerMock().Object, Mock.Of<ITokenService>(), context, Mock.Of<IConfiguration>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.RefreshToken(new TokenRefreshRequestDto { RefreshToken = "expired-token" }));
    }

    [Fact]
    public async Task RefreshToken_WhenTokenIsRevoked_ThrowsUnauthorizedAccessException()
    {
        var context = CreateContext();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        context.Users.Add(user);
        context.RefreshTokens.Add(new RefreshToken
        {
            Token = "revoked-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = true
        });
        await context.SaveChangesAsync();

        var sut = new AuthService(CreateUserManagerMock().Object, Mock.Of<ITokenService>(), context, Mock.Of<IConfiguration>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => sut.RefreshToken(new TokenRefreshRequestDto { RefreshToken = "revoked-token" }));
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_RotatesTokenAndRevokesOldOne()
    {
        var context = CreateContext();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        context.Users.Add(user);
        context.RefreshTokens.Add(new RefreshToken
        {
            Token = "old-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        });
        await context.SaveChangesAsync();

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.CreateToken(It.IsAny<AppUser>(), It.IsAny<IEnumerable<string>>())).Returns("new-access-token");
        tokenService.Setup(t => t.GenerateRefreshToken()).Returns("new-refresh-token");

        var sut = new AuthService(CreateUserManagerMock().Object, tokenService.Object, context, Mock.Of<IConfiguration>());

        var result = await sut.RefreshToken(new TokenRefreshRequestDto { RefreshToken = "old-token" });

        Assert.Equal("new-access-token", GetPropertyValue(result, "accessToken"));
        Assert.Equal("new-refresh-token", GetPropertyValue(result, "refreshToken"));

        var oldToken = await context.RefreshTokens.SingleAsync(t => t.Token == "old-token");
        Assert.True(oldToken.IsRevoked);

        var newToken = await context.RefreshTokens.SingleAsync(t => t.Token == "new-refresh-token");
        Assert.False(newToken.IsRevoked);
    }

    [Fact]
    public async Task Logout_WithValidToken_RevokesIt()
    {
        var context = CreateContext();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };
        context.Users.Add(user);
        context.RefreshTokens.Add(new RefreshToken
        {
            Token = "active-token",
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        });
        await context.SaveChangesAsync();

        var sut = new AuthService(CreateUserManagerMock().Object, Mock.Of<ITokenService>(), context, Mock.Of<IConfiguration>());

        await sut.Logout("active-token");

        var token = await context.RefreshTokens.SingleAsync(t => t.Token == "active-token");
        Assert.True(token.IsRevoked);
    }

    [Fact]
    public async Task Logout_WithUnknownToken_DoesNotThrow()
    {
        var sut = new AuthService(CreateUserManagerMock().Object, Mock.Of<ITokenService>(), CreateContext(), Mock.Of<IConfiguration>());

        await sut.Logout("does-not-exist");
    }
}
