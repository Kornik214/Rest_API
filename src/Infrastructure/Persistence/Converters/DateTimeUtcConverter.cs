using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.Converters;

public sealed class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeUtcConverter()
        : base(
            src => src.Kind == DateTimeKind.Utc ? src : src.ToUniversalTime(),
            db => db.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(db, DateTimeKind.Utc)
                : db.ToUniversalTime())
    { }
}