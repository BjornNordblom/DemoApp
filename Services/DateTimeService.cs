namespace DemoApp.Services;

public class DateTimeService : IDateTimeService
{
    private Func<DateTime> _utcNow;

    public DateTime UtcNow => _utcNow();

    public DateTimeService()
    {
        _utcNow = () => DateTime.UtcNow;
    }

    public DateTimeService(Func<DateTime> utcNow)
    {
        if (utcNow is null)
        {
            throw new ArgumentNullException(nameof(utcNow));
        }
        _utcNow = utcNow;
    }
}
