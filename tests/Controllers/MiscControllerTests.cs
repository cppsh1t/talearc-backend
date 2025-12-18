using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.Tests.Controllers;

public class MiscControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public MiscControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateMisc_ShouldSaveMiscToDatabase()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);

        var worldView = new WorldView
        {
            UserId = user.Id,
            Name = "Test World",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);
        await _context.SaveChangesAsync();

        var misc = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Misc",
            Description = "Test Description",
            Type = "道具",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.Miscs.Add(misc);
        await _context.SaveChangesAsync();

        // Assert
        var savedMisc = await _context.Miscs.FirstOrDefaultAsync(m => m.Id == misc.Id);
        Assert.NotNull(savedMisc);
        Assert.Equal("Test Misc", savedMisc.Name);
        Assert.Equal("Test Description", savedMisc.Description);
        Assert.Equal("道具", savedMisc.Type);
        Assert.Equal(user.Id, savedMisc.UserId);
        Assert.Equal(worldView.Id, savedMisc.WorldViewId);
    }

    [Fact]
    public async Task GetMisc_ShouldReturnMiscById()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);

        var worldView = new WorldView
        {
            UserId = user.Id,
            Name = "Test World",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);

        var misc = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Misc",
            Description = "Test Description",
            Type = "地点",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Miscs.Add(misc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Miscs
            .FirstOrDefaultAsync(m => m.Id == misc.Id && m.UserId == user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Misc", result.Name);
        Assert.Equal("地点", result.Type);
    }

    [Fact]
    public async Task UpdateMisc_ShouldModifyMiscProperties()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);

        var worldView = new WorldView
        {
            UserId = user.Id,
            Name = "Test World",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);

        var misc = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Original Name",
            Description = "Original Description",
            Type = "道具",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Miscs.Add(misc);
        await _context.SaveChangesAsync();

        // Act
        misc.Name = "Updated Name";
        misc.Description = "Updated Description";
        misc.Type = "事件";
        misc.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedMisc = await _context.Miscs.FirstOrDefaultAsync(m => m.Id == misc.Id);
        Assert.NotNull(updatedMisc);
        Assert.Equal("Updated Name", updatedMisc.Name);
        Assert.Equal("Updated Description", updatedMisc.Description);
        Assert.Equal("事件", updatedMisc.Type);
    }

    [Fact]
    public async Task DeleteMisc_ShouldRemoveMiscFromDatabase()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);

        var worldView = new WorldView
        {
            UserId = user.Id,
            Name = "Test World",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);

        var misc = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Misc",
            Description = "Test Description",
            Type = "道具",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Miscs.Add(misc);
        await _context.SaveChangesAsync();

        var miscId = misc.Id;

        // Act
        _context.Miscs.Remove(misc);
        await _context.SaveChangesAsync();

        // Assert
        var deletedMisc = await _context.Miscs.FirstOrDefaultAsync(m => m.Id == miscId);
        Assert.Null(deletedMisc);
    }

    [Fact]
    public async Task GetMiscsByWorldView_ShouldFilterByWorldViewId()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);

        var worldView1 = new WorldView
        {
            UserId = user.Id,
            Name = "World 1",
            Description = "Description 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var worldView2 = new WorldView
        {
            UserId = user.Id,
            Name = "World 2",
            Description = "Description 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.AddRange(worldView1, worldView2);
        await _context.SaveChangesAsync();

        var misc1 = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView1.Id,
            Name = "Misc 1",
            Type = "道具",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var misc2 = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView2.Id,
            Name = "Misc 2",
            Type = "地点",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Miscs.AddRange(misc1, misc2);
        await _context.SaveChangesAsync();

        // Act
        var miscsInWorld1 = await _context.Miscs
            .Where(m => m.UserId == user.Id && m.WorldViewId == worldView1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(miscsInWorld1);
        Assert.Equal("Misc 1", miscsInWorld1[0].Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
