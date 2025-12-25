using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data;
using talearc_backend.src.data.entities;

namespace talearc_backend.Tests.Controllers;

public class CharacterControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public CharacterControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CreateCharacter_ShouldSaveCharacterToDatabase()
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

        var character = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView.Id,
            Name = "Test Character",
            Description = "Test Description",
            Note = "Test Note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        // Assert
        var savedCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == character.Id);
        Assert.NotNull(savedCharacter);
        Assert.Equal("Test Character", savedCharacter.Name);
        Assert.Equal("Test Description", savedCharacter.Description);
        Assert.Equal("Test Note", savedCharacter.Note);
        Assert.Equal(user.Id, savedCharacter.UserId);
        Assert.Equal(worldView.Id, savedCharacter.WorldViewId);
    }

    [Fact]
    public async Task GetCharacter_ShouldReturnCharacterById()
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
            Description = "Test Description",
            Note = "Test Note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Characters
            .FirstOrDefaultAsync(c => c.Id == character.Id && c.UserId == user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Character", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task UpdateCharacter_ShouldModifyCharacterProperties()
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
            Name = "Original Name",
            Description = "Original Description",
            Note = "Original Note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        // Act
        character.Name = "Updated Name";
        character.Description = "Updated Description";
        character.Note = "Updated Note";
        character.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Assert
        var updatedCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == character.Id);
        Assert.NotNull(updatedCharacter);
        Assert.Equal("Updated Name", updatedCharacter.Name);
        Assert.Equal("Updated Description", updatedCharacter.Description);
        Assert.Equal("Updated Note", updatedCharacter.Note);
    }

    [Fact]
    public async Task DeleteCharacter_ShouldRemoveCharacterFromDatabase()
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
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        var characterId = character.Id;

        // Act
        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();

        // Assert
        var deletedCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == characterId);
        Assert.Null(deletedCharacter);
    }

    [Fact]
    public async Task GetCharactersByWorldView_ShouldFilterByWorldViewId()
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
            Description = "Description 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var character2 = new Character
        {
            UserId = user.Id,
            WorldViewId = worldView2.Id,
            Name = "Character 2",
            Description = "Description 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.AddRange(character1, character2);
        await _context.SaveChangesAsync();

        // Act
        var charactersInWorld1 = await _context.Characters
            .Where(c => c.UserId == user.Id && c.WorldViewId == worldView1.Id)
            .ToListAsync();

        // Assert
        Assert.Single(charactersInWorld1);
        Assert.Equal("Character 1", charactersInWorld1[0].Name);
    }

    [Fact]
    public async Task Character_ShouldHaveSnapshotIds()
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
            SnapshotIds = new[] { 1, 2, 3 },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        // Act
        var savedCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == character.Id);

        // Assert
        Assert.NotNull(savedCharacter);
        Assert.Equal(3, savedCharacter.SnapshotIds.Length);
        Assert.Contains(1, savedCharacter.SnapshotIds);
        Assert.Contains(2, savedCharacter.SnapshotIds);
        Assert.Contains(3, savedCharacter.SnapshotIds);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
