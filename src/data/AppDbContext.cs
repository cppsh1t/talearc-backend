using Microsoft.EntityFrameworkCore;
using talearc_backend.src.data.entities;

namespace talearc_backend.src.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Person> Person { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasNoKey();
    }
}