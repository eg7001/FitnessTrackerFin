using System.IdentityModel.Tokens.Jwt;
using FitnessTracker.Models;
using FitnessTracker.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitnessTracker.Tests.Services;

public class TokenServiceTests
{
    // TokenService only ever reads config through the indexer (e.g.
    // configuration["Jwt:Key"]), so a Moq mock of IConfiguration is enough -
    // no need to spin up a real configuration provider for this.
    private static TokenService CreateSut(int expiresInMinutes = 60)
    {
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Jwt:Key"]).Returns("unit-test-signing-key-that-is-long-enough-1234567890");
        configuration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        configuration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        configuration.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns(expiresInMinutes.ToString());

        return new TokenService(configuration.Object);
    }

    [Fact]
    public void CreateToken_IncludesExpectedClaimsAndIssuerAudience()
    {
        var sut = CreateSut();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };

        var token = sut.CreateToken(user, Array.Empty<string>());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("TestIssuer", jwt.Issuer);
        Assert.Contains(jwt.Audiences, a => a == "TestAudience");
        Assert.Contains(jwt.Claims, c => c.Type == "email" && c.Value == "test@example.com");
        Assert.Contains(jwt.Claims, c => c.Type == "sub" && c.Value == user.Id.ToString());
    }

    [Fact]
    public void CreateToken_SetsExpirationBasedOnConfiguredMinutes()
    {
        var sut = CreateSut(expiresInMinutes: 30);
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };

        var token = sut.CreateToken(user, Array.Empty<string>());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        Assert.True(Math.Abs((jwt.ValidTo - expectedExpiry).TotalMinutes) < 1);
    }

    [Fact]
    public void CreateToken_WhenRolesProvided_IncludesARoleClaimPerRole()
    {
        // [Authorize(Roles = "Admin")] on the Exercises endpoints checks
        // ClaimTypes.Role on the token, not the database - if this claim
        // is missing, an Admin user would still get 403'd everywhere.
        var sut = CreateSut();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };

        var token = sut.CreateToken(user, new[] { "Admin", "Member" });
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        var roleClaims = jwt.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
        Assert.Contains("Admin", roleClaims);
        Assert.Contains("Member", roleClaims);
    }

    [Fact]
    public void CreateToken_WhenNoRoles_IncludesNoRoleClaims()
    {
        var sut = CreateSut();
        var user = new AppUser { Id = Guid.NewGuid(), Email = "test@example.com", UserName = "test@example.com" };

        var token = sut.CreateToken(user, Array.Empty<string>());
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.DoesNotContain(jwt.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsANonEmptyUniqueValueEachTime()
    {
        var sut = CreateSut();

        var token1 = sut.GenerateRefreshToken();
        var token2 = sut.GenerateRefreshToken();

        Assert.False(string.IsNullOrWhiteSpace(token1));
        Assert.NotEqual(token1, token2);
    }
}
