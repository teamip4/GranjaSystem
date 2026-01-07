using GrajaSystemProject.Models.User;
using GranjaSystemProject.Models.Farm;
using Microsoft.EntityFrameworkCore;

namespace GrajaSystemProject.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Farm> Farms { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Type).HasConversion<string>();
        });
    }
}
