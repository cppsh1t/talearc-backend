using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.Tests.Controllers;

public class WorldEventControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public WorldEventControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateWorldEvent_ShouldSaveWorldEventToDatabase()
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

        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Event",
            Description = "Test Event Description",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [1, 2, 3],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.WorldEvents.Add(worldEvent);
        await _context.SaveChangesAsync();

        // Assert
        var savedWorldEvent = await _context.WorldEvents.FirstOrDefaultAsync(e => e.Id == worldEvent.Id);
        Assert.NotNull(savedWorldEvent);
        Assert.Equal("Test Event", savedWorldEvent.Name);
        Assert.Equal("Test Event Description", savedWorldEvent.Description);
        Assert.Equal(user.Id, savedWorldEvent.UserId);
        Assert.Equal(worldView.Id, savedWorldEvent.WorldViewId);
        Assert.Equal(3, savedWorldEvent.RelatedCharacterSnapshotIds.Length);
    }

    [Fact]
    public async Task GetWorldEvent_ShouldReturnWorldEventById()
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

        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Event",
            Description = "Test Description",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.Add(worldEvent);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.WorldEvents
            .FirstOrDefaultAsync(e => e.Id == worldEvent.Id && e.UserId == user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Event", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task UpdateWorldEvent_ShouldModifyWorldEventProperties()
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

        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Original Name",
            Description = "Original Description",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [1],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.Add(worldEvent);
        await _context.SaveChangesAsync();

        // Act
        worldEvent.Name = "Updated Name";
        worldEvent.Description = "Updated Description";
        worldEvent.RelatedCharacterSnapshotIds = [1, 2, 3];
        worldEvent.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedWorldEvent = await _context.WorldEvents.FirstOrDefaultAsync(e => e.Id == worldEvent.Id);
        Assert.NotNull(updatedWorldEvent);
        Assert.Equal("Updated Name", updatedWorldEvent.Name);
        Assert.Equal("Updated Description", updatedWorldEvent.Description);
        Assert.Equal(3, updatedWorldEvent.RelatedCharacterSnapshotIds.Length);
    }

    [Fact]
    public async Task DeleteWorldEvent_ShouldRemoveWorldEventFromDatabase()
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

        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Event",
            Description = "Test Description",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.Add(worldEvent);
        await _context.SaveChangesAsync();

        var worldEventId = worldEvent.Id;

        // Act
        _context.WorldEvents.Remove(worldEvent);
        await _context.SaveChangesAsync();

        // Assert
        var deletedWorldEvent = await _context.WorldEvents.FirstOrDefaultAsync(e => e.Id == worldEventId);
        Assert.Null(deletedWorldEvent);
    }

    [Fact]
    public async Task GetWorldEventsByWorldView_ShouldFilterByWorldViewId()
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

        var worldEvent1 = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView1.Id,
            Name = "Event 1",
            Description = "Description 1",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var worldEvent2 = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView2.Id,
            Name = "Event 2",
            Description = "Description 2",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.AddRange(worldEvent1, worldEvent2);
        await _context.SaveChangesAsync();

        // Act
        var eventsInWorld1 = await _context.WorldEvents
            .Where(e => e.UserId == user.Id && e.WorldViewId == worldView1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(eventsInWorld1);
        Assert.Equal("Event 1", eventsInWorld1[0].Name);
    }

    [Fact]
    public async Task WorldEvent_ShouldHandleEmptyRelatedCharacterSnapshotIds()
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

        var worldEvent = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Event",
            Description = "Test Description",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.Add(worldEvent);
        await _context.SaveChangesAsync();

        // Act
        var savedEvent = await _context.WorldEvents.FirstOrDefaultAsync(e => e.Id == worldEvent.Id);

        // Assert
        Assert.NotNull(savedEvent);
        Assert.NotNull(savedEvent.RelatedCharacterSnapshotIds);
        Assert.Empty(savedEvent.RelatedCharacterSnapshotIds);
    }

    [Fact]
    public async Task GetAllWorldEventsByWorldViewId_ShouldReturnAllEventsForWorldView()
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

        var event1 = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Event 1",
            Description = "Description 1",
            HappenedAt = DateTime.UtcNow.AddDays(-2),
            EndAt = DateTime.UtcNow.AddDays(-1),
            RelatedCharacterSnapshotIds = [1],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var event2 = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Event 2",
            Description = "Description 2",
            HappenedAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddDays(1),
            RelatedCharacterSnapshotIds = [2, 3],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var event3 = new WorldEvent
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Event 3",
            Description = "Description 3",
            HappenedAt = DateTime.UtcNow.AddDays(-5),
            EndAt = DateTime.UtcNow.AddDays(-4),
            RelatedCharacterSnapshotIds = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorldEvents.AddRange(event1, event2, event3);
        await _context.SaveChangesAsync();

        // Act
        var allEvents = await _context.WorldEvents
            .Where(e => e.WorldViewId == worldView.Id && e.UserId == user.Id)
            .OrderByDescending(e => e.HappenedAt)
            .ToListAsync();

        // Assert
        Assert.Equal(3, allEvents.Count);
        Assert.Equal("Event 2", allEvents[0].Name);
        Assert.Equal("Event 1", allEvents[1].Name);
        Assert.Equal("Event 3", allEvents[2].Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
