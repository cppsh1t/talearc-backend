using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data.entities;

namespace talearc_backend.src.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterSnapshot> CharacterSnapshots { get; set; }
    public DbSet<Misc> Miscs { get; set; }
    public DbSet<WorldEvent> WorldEvents { get; set; }
    public DbSet<WorldView> WorldViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreateAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}