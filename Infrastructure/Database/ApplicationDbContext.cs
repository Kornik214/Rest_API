using Domain.Scooters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Scooter> Scooters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Автоматично застосовує всі класи, що реалізують IEntityTypeConfiguration зі збірки
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}