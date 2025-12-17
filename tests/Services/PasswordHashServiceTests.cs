using talearc_backend.src.application.service;

namespace talearc_backend.Tests.Services;

public class PasswordHashServiceTests
{
    private readonly PasswordHashService _passwordHashService;

    public PasswordHashServiceTests()
    {
        _passwordHashService = new PasswordHashService();
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnNonEmptyHash()
    {
        // Arrange
        var password = "testPassword123";

        // Act
        var hash = await _passwordHashService.HashPasswordAsync(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldGenerateDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "testPassword123";

        // Act
        var hash1 = await _passwordHashService.HashPasswordAsync(password);
        var hash2 = await _passwordHashService.HashPasswordAsync(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // 因为每次生成的盐不同
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnTrue_WhenPasswordMatches()
    {
        // Arrange
        var password = "testPassword123";
        var hash = await _passwordHashService.HashPasswordAsync(password);

        // Act
        var result = await _passwordHashService.VerifyPasswordAsync(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var password = "testPassword123";
        var wrongPassword = "wrongPassword";
        var hash = await _passwordHashService.HashPasswordAsync(password);

        // Act
        var result = await _passwordHashService.VerifyPasswordAsync(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldReturnFalse_WhenHashIsInvalid()
    {
        // Arrange
        var password = "testPassword123";
        var invalidHash = "invalidHash";

        // Act
        var result = await _passwordHashService.VerifyPasswordAsync(password, invalidHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_ShouldHandleEmptyPassword()
    {
        // Arrange
        var password = "";
        var hash = await _passwordHashService.HashPasswordAsync(password);

        // Act
        var result = await _passwordHashService.VerifyPasswordAsync(password, hash);

        // Assert
        Assert.True(result);
    }
}
