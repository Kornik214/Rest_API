using Domain.Scooters;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ScootersConfiguration : IEntityTypeConfiguration<Scooter>
{
    public void Configure(EntityTypeBuilder<Scooter> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("uuid").IsRequired();
        builder.Property(x => x.Model).HasColumnType("varchar(100)").IsRequired();
        builder.Property(x => x.SerialNumber).HasColumnType("varchar(50)").IsRequired();
        builder.HasIndex(x => x.SerialNumber).IsUnique();
        builder.Property(x => x.BatteryPercent).IsRequired();
        builder.Property(x => x.PricePerMinute).HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(x => x.IsAvailable).IsRequired();
        builder.Property(x => x.LastServiceDate)
            .HasConversion(new DateTimeUtcConverter())
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}
