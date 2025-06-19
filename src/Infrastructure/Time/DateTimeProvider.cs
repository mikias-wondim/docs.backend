using SharedKernel;

namespace Infrastructure.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime GetNow => DateTime.UtcNow;
}
