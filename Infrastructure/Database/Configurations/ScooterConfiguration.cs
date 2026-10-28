using Domain.Scooters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ScooterConfiguration : IEntityTypeConfiguration<Scooter>
{
    public void Configure(EntityTypeBuilder<Scooter> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new ScooterId(value));
        
        builder.Property(s => s.SerialNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.SerialNumber)
            .IsUnique();

        builder.Property(s => s.BatteryLevel)
            .IsRequired();

        builder.Property(s => s.IsAvailable)
            .IsRequired();
        
        builder.Property(s => s.LastMaintenanceDate)
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        
        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnAdd()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}