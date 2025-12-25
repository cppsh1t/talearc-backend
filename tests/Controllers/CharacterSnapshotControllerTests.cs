using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.Tests.Controllers;

public class CharacterSnapshotControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public CharacterSnapshotControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCharacterSnapshot_ShouldSaveSnapshotToDatabase()
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

        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        var snapshot = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character.Id,
            Name = "Test Snapshot",
            Description = "Test Description",
            Note = "Test Note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.CharacterSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        // Assert
        var savedSnapshot = await _context.CharacterSnapshots.FirstOrDefaultAsync(s => s.Id == snapshot.Id);
        Assert.NotNull(savedSnapshot);
        Assert.Equal("Test Snapshot", savedSnapshot.Name);
        Assert.Equal("Test Description", savedSnapshot.Description);
        Assert.Equal("Test Note", savedSnapshot.Note);
        Assert.Equal(user.Id, savedSnapshot.UserId);
        Assert.Equal(worldView.Id, savedSnapshot.WorldViewId);
        Assert.Equal(character.Id, savedSnapshot.CharacterId);
    }

    [Fact]
    public async Task GetCharacterSnapshot_ShouldReturnSnapshotById()
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

        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);

        var snapshot = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character.Id,
            Name = "Test Snapshot",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CharacterSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.CharacterSnapshots
            .FirstOrDefaultAsync(s => s.Id == snapshot.Id && s.UserId == user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Snapshot", result.Name);
        Assert.Equal(character.Id, result.CharacterId);
    }

    [Fact]
    public async Task UpdateCharacterSnapshot_ShouldModifySnapshotProperties()
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

        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);

        var snapshot = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character.Id,
            Name = "Original Name",
            Description = "Original Description",
            Note = "Original Note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CharacterSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        // Act
        snapshot.Name = "Updated Name";
        snapshot.Description = "Updated Description";
        snapshot.Note = "Updated Note";
        snapshot.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedSnapshot = await _context.CharacterSnapshots.FirstOrDefaultAsync(s => s.Id == snapshot.Id);
        Assert.NotNull(updatedSnapshot);
        Assert.Equal("Updated Name", updatedSnapshot.Name);
        Assert.Equal("Updated Description", updatedSnapshot.Description);
        Assert.Equal("Updated Note", updatedSnapshot.Note);
    }

    [Fact]
    public async Task DeleteCharacterSnapshot_ShouldRemoveSnapshotFromDatabase()
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

        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);

        var snapshot = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character.Id,
            Name = "Test Snapshot",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CharacterSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        var snapshotId = snapshot.Id;

        // Act
        _context.CharacterSnapshots.Remove(snapshot);
        await _context.SaveChangesAsync();

        // Assert
        var deletedSnapshot = await _context.CharacterSnapshots.FirstOrDefaultAsync(s => s.Id == snapshotId);
        Assert.Null(deletedSnapshot);
    }

    [Fact]
    public async Task GetSnapshotsByCharacter_ShouldFilterByCharacterId()
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

        var character1 = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Character 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var character2 = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Character 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.AddRange(character1, character2);
        await _context.SaveChangesAsync();

        var snapshot1 = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character1.Id,
            Name = "Snapshot 1",
            Description = "Description 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var snapshot2 = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            CharacterId = character2.Id,
            Name = "Snapshot 2",
            Description = "Description 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CharacterSnapshots.AddRange(snapshot1, snapshot2);
        await _context.SaveChangesAsync();

        // Act
        var snapshotsForCharacter1 = await _context.CharacterSnapshots
            .Where(s => s.UserId == user.Id && s.CharacterId == character1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(snapshotsForCharacter1);
        Assert.Equal("Snapshot 1", snapshotsForCharacter1[0].Name);
    }

    [Fact]
    public async Task GetSnapshotsByWorldView_ShouldFilterByWorldViewId()
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

        var character1 = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView1.Id,
            Name = "Character 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var character2 = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView2.Id,
            Name = "Character 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.AddRange(character1, character2);
        await _context.SaveChangesAsync();

        var snapshot1 = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView1.Id,
            CharacterId = character1.Id,
            Name = "Snapshot 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var snapshot2 = new CharacterSnapshot
        {
            UserId = user.Id,
            WorldViewId = worldView2.Id,
            CharacterId = character2.Id,
            Name = "Snapshot 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.CharacterSnapshots.AddRange(snapshot1, snapshot2);
        await _context.SaveChangesAsync();

        // Act
        var snapshotsInWorld1 = await _context.CharacterSnapshots
            .Where(s => s.UserId == user.Id && s.WorldViewId == worldView1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(snapshotsInWorld1);
        Assert.Equal("Snapshot 1", snapshotsInWorld1[0].Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
