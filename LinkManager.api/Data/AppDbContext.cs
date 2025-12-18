using LinkManager.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkManager.Api.Data;

public class AppDbContext : DbContext {
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Link> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PageSlug)
            .IsUnique();
    }
}