using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using talearc_backend.src.application.controllers.auth;
using talearc_backend.src.application.service;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;
using talearc_backend.src.structure;

namespace talearc_backend.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly PasswordHashService _passwordHashService;
    private readonly Mock<RegistrationKeyService> _mockRegistrationKeyService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // 使用 InMemory 数据库
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _mockLogger = new Mock<ILogger<AuthController>>();
        
        // 为JwtTokenGenerator创建Mock配置
        var mockConfig = new Mock<IConfiguration>();
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("test-secret-key-minimum-32-chars-required-for-security");
        mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns("test-secret-key-minimum-32-chars-required-for-security");
        mockConfig.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");
        _tokenGenerator = new JwtTokenGenerator(mockConfig.Object);
        
        _passwordHashService = new PasswordHashService();
        _mockRegistrationKeyService = new Mock<RegistrationKeyService>(null!, null!);

        _controller = new AuthController(
            _context,
            _mockLogger.Object,
            _tokenGenerator,
            _passwordHashService,
            _mockRegistrationKeyService.Object
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var loginForm = new LoginForm
        {
            Name = "nonexistent",
            Password = "password123"
        };

        // Act
        var result = await _controller.Login(loginForm);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
        Assert.Equal(401, response.Code);
        Assert.Equal("登录信息错误", response.Message);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
    {
        // Arrange
        var hashedPassword = await _passwordHashService.HashPasswordAsync("correctpassword");
        var user = new User
        {
            Name = "testuser",
            Password = hashedPassword,
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginForm = new LoginForm
        {
            Name = "testuser",
            Password = "wrongpassword"
        };

        // Act
        var result = await _controller.Login(loginForm);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
        Assert.Equal(401, response.Code);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var password = "password123";
        var hashedPassword = await _passwordHashService.HashPasswordAsync(password);
        var user = new User
        {
            Name = "testuser",
            Password = hashedPassword,
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginForm = new LoginForm
        {
            Name = "testuser",
            Password = password
        };

        // 不需要Setup，真实的_tokenGenerator会生成token

        // Act
        var result = await _controller.Login(loginForm);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<talearc_backend.src.data.dto.LoginResponseDto>>(okResult.Value);
        Assert.Equal(200, response.Code);
        Assert.Equal("登录成功", response.Message);
    }
}
