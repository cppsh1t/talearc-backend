using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.Tests.Controllers;

public class WorldViewControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public WorldViewControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateWorldView_ShouldSaveWorldViewToDatabase()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var worldView = new WorldView
        {
            UserId = user.Id,
            Name = "Test World",
            Description = "Test Description",
            Notes = "Test Notes",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.WorldViews.Add(worldView);
        await _context.SaveChangesAsync();

        // Assert
        var savedWorldView = await _context.WorldViews.FirstOrDefaultAsync(w => w.Id == worldView.Id);
        Assert.NotNull(savedWorldView);
        Assert.Equal("Test World", savedWorldView.Name);
        Assert.Equal("Test Description", savedWorldView.Description);
        Assert.Equal("Test Notes", savedWorldView.Notes);
        Assert.Equal(user.Id, savedWorldView.UserId);
    }

    [Fact]
    public async Task GetWorldView_ShouldReturnWorldViewById()
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
            Notes = "Test Notes",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.WorldViews
            .FirstOrDefaultAsync(w => w.Id == worldView.Id && w.UserId == user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test World", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("Test Notes", result.Notes);
    }

    [Fact]
    public async Task UpdateWorldView_ShouldModifyWorldViewProperties()
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
            Name = "Original Name",
            Description = "Original Description",
            Notes = "Original Notes",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.Add(worldView);
        await _context.SaveChangesAsync();

        // Act
        worldView.Name = "Updated Name";
        worldView.Description = "Updated Description";
        worldView.Notes = "Updated Notes";
        worldView.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedWorldView = await _context.WorldViews.FirstOrDefaultAsync(w => w.Id == worldView.Id);
        Assert.NotNull(updatedWorldView);
        Assert.Equal("Updated Name", updatedWorldView.Name);
        Assert.Equal("Updated Description", updatedWorldView.Description);
        Assert.Equal("Updated Notes", updatedWorldView.Notes);
    }

    [Fact]
    public async Task DeleteWorldView_ShouldRemoveWorldViewFromDatabase()
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

        var worldViewId = worldView.Id;

        // Act
        _context.WorldViews.Remove(worldView);
        await _context.SaveChangesAsync();

        // Assert
        var deletedWorldView = await _context.WorldViews.FirstOrDefaultAsync(w => w.Id == worldViewId);
        Assert.Null(deletedWorldView);
    }

    [Fact]
    public async Task GetWorldViewsByUser_ShouldFilterByUserId()
    {
        // Arrange
        var user1 = new User
        {
            Name = "testuser1",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        var user2 = new User
        {
            Name = "testuser2",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var worldView1 = new WorldView
        {
            UserId = user1.Id,
            Name = "World 1",
            Description = "Description 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var worldView2 = new WorldView
        {
            UserId = user2.Id,
            Name = "World 2",
            Description = "Description 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.AddRange(worldView1, worldView2);
        await _context.SaveChangesAsync();

        // Act
        var worldViewsForUser1 = await _context.WorldViews
            .Where(w => w.UserId == user1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(worldViewsForUser1);
        Assert.Equal("World 1", worldViewsForUser1[0].Name);
    }

    [Fact]
    public async Task WorldView_ShouldHaveDefaultEmptyArrays()
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

        // Act
        var savedWorldView = await _context.WorldViews.FirstOrDefaultAsync(w => w.Id == worldView.Id);

        // Assert
        Assert.NotNull(savedWorldView);
        Assert.NotNull(savedWorldView.CharacterIds);
        Assert.Empty(savedWorldView.CharacterIds);
        Assert.NotNull(savedWorldView.MiscIds);
        Assert.Empty(savedWorldView.MiscIds);
        Assert.NotNull(savedWorldView.WorldEventIds);
        Assert.Empty(savedWorldView.WorldEventIds);
    }

    [Fact]
    public async Task DeleteWorldView_ShouldCascadeDeleteRelatedEntities()
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

        // 创建关联的角色
        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);

        // 创建关联的世界事件
        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Event",
            HappenedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.Add(worldEvent);

        // 创建关联的杂项
        var misc = new Misc
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Misc",
            Type = "道具",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Miscs.Add(misc);

        await _context.SaveChangesAsync();

        var worldViewId = worldView.Id;

        // Act - 删除世界观及其所有关联实体
        var characters = await _context.Characters.Where(c => c.WorldViewId == worldViewId).ToListAsync();
        _context.Characters.RemoveRange(characters);

        var worldEvents = await _context.WorldEvents.Where(e => e.WorldViewId == worldViewId).ToListAsync();
        _context.WorldEvents.RemoveRange(worldEvents);

        var miscs = await _context.Miscs.Where(m => m.WorldViewId == worldViewId).ToListAsync();
        _context.Miscs.RemoveRange(miscs);

        _context.WorldViews.Remove(worldView);
        await _context.SaveChangesAsync();

        // Assert
        var deletedWorldView = await _context.WorldViews.FirstOrDefaultAsync(w => w.Id == worldViewId);
        Assert.Null(deletedWorldView);

        var deletedCharacters = await _context.Characters.Where(c => c.WorldViewId == worldViewId).ToListAsync();
        Assert.Empty(deletedCharacters);

        var deletedWorldEvents = await _context.WorldEvents.Where(e => e.WorldViewId == worldViewId).ToListAsync();
        Assert.Empty(deletedWorldEvents);

        var deletedMiscs = await _context.Miscs.Where(m => m.WorldViewId == worldViewId).ToListAsync();
        Assert.Empty(deletedMiscs);
    }

    [Fact]
    public async Task WorldView_ShouldSupportMultipleWorldViewsPerUser()
    {
        // Arrange
        var user = new User
        {
            Name = "testuser",
            Password = "hash",
            CreateAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var worldView1 = new WorldView
        {
            UserId = user.Id,
            Name = "Fantasy World",
            Description = "A magical realm",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var worldView2 = new WorldView
        {
            UserId = user.Id,
            Name = "Sci-Fi World",
            Description = "A futuristic universe",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldViews.AddRange(worldView1, worldView2);
        await _context.SaveChangesAsync();

        // Act
        var userWorldViews = await _context.WorldViews
            .Where(w => w.UserId == user.Id)
            .ToListAsync();

        // Assert
        Assert.Equal(2, userWorldViews.Count);
        Assert.Contains(userWorldViews, w => w.Name == "Fantasy World");
        Assert.Contains(userWorldViews, w => w.Name == "Sci-Fi World");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
