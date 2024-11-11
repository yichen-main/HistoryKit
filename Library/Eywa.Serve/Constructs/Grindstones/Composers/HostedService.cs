namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class HostedService : BackgroundService
{
    const int initialSeconds = 1;
    protected enum Interval
    {
        Instant = 1,
        TenSeconds = 10,
        Minute = 60,
        Hour = 3600,
        QuarterHour = 3600 / 4,
        Daily = 3600 * 24,
    }
    protected async Task ActionAsync(Type type, Func<ValueTask> task)
    {
        try
        {
            await task().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            e.Error(type);
        }
    }
    protected Task ActionAsync<T>(in T content, in Func<ValueTask> task) where T : HostedService
    {
        return ActionAsync(content.GetType(), task);
    }
    protected async Task KeepAsync<T>(T content, Func<ValueTask> task, Action<Exception>? e = default, bool initialSkip = default) where T : HostedService
    {
        var type = content.GetType();
        if (!Chargers.Any(x => x.Key.IsMatch(type.Name))) Chargers[type.Name] = initialSeconds;
        PeriodicTimer initialTimer = new(TimeSpan.FromTicks(TimeSpan.TicksPerSecond * Interval.Instant.GetHashCode()));
        while (await initialTimer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            try
            {
                var seconds = initialSeconds;
                if (!ExistArchway(type.Name))
                {
                    if (initialSkip) seconds = Chargers[type.Name];
                }
                else seconds = Chargers[type.Name];
                PeriodicTimer timer = new(TimeSpan.FromTicks(TimeSpan.TicksPerSecond * seconds));
                while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
                {
                    try
                    {
                        await task().ConfigureAwait(false);
                        if (seconds != Chargers[type.Name]) timer.Dispose();
                        Histories.TryRemove(type.Name, out _);
                    }
                    catch (Exception exception)
                    {
                        if (e is not null) e(exception);
                        if (Histories.TryGetValue(type.Name, out var value))
                        {
                            if (!value.IsMatch(exception.Message)) exception.Error(type);
                        }
                        else exception.Error(type);
                        Histories[type.Name] = exception.Message;
                    }
                }
            }
            catch (Exception exception)
            {
                if (e is not null) e(exception);
                exception.Fatal(type);
            }
        }
        static bool ExistArchway(in string flag)
        {
            try
            {
                ReaderWriterLock.EnterReadLock();
                return IdentifyFlags.Contains(flag);
            }
            finally
            {
                ReaderWriterLock.ExitReadLock();
                Refresh(flag);
            }
            static void Refresh(in string flag)
            {
                try
                {
                    ReaderWriterLock.EnterWriteLock();
                    IdentifyFlags.Add(flag);
                }
                finally
                {
                    ReaderWriterLock.ExitWriteLock();
                }
            }
        }
    }
    protected static void SetInterval<T>(in T content, in Interval type) where T : notnull
    {
        Chargers[content.GetType().Name] = type.GetHashCode();
    }
    protected void UseCronJob<T>(in string cronExpression) where T : IInvocable => Scheduler.Schedule<T>().Cron(cronExpression);
    protected void UseSecondJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EverySecond();
    protected void UseFiveSecondsJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryFiveSeconds();
    protected void UseTenSecondsJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryTenSeconds();
    protected void UseFifteenSecondsJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryFifteenSeconds();
    protected void UseThirtySecondsJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryThirtySeconds();
    protected void UseSecondsJob<T>(in int seconds) where T : IInvocable => Scheduler.Schedule<T>().EverySeconds(seconds);
    protected void UseMinuteJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryMinute();
    protected void UseFiveMinutesJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryFiveMinutes();
    protected void UseTenMinutesJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryTenMinutes();
    protected void UseFifteenMinutesJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryFifteenMinutes();
    protected void UseThirtyMinutesJob<T>() where T : IInvocable => Scheduler.Schedule<T>().EveryThirtyMinutes();
    protected void UseHourlyJob<T>() where T : IInvocable => Scheduler.Schedule<T>().Hourly();
    protected void UseHourlyAtJob<T>(in int minute) where T : IInvocable => Scheduler.Schedule<T>().HourlyAt(minute);
    protected void UseDailyJob<T>() where T : IInvocable => Scheduler.Schedule<T>().Daily();
    protected void UseDailyAtJob<T>(in int hour) where T : IInvocable => Scheduler.Schedule<T>().DailyAtHour(hour);
    protected void UseDailyAtJob<T>(in int hour, in int minute) where T : IInvocable => Scheduler.Schedule<T>().DailyAt(hour, minute);
    protected void UseWeeklyJob<T>() where T : IInvocable => Scheduler.Schedule<T>().Weekly();
    protected void UseMonthlyJob<T>() where T : IInvocable => Scheduler.Schedule<T>().Monthly();
    public required IScheduler Scheduler { get; init; }
    public required IDurableSetup DurableSetup { get; init; }
    public required ITimelineData TimelineData { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
    static ConcurrentDictionary<string, int> Chargers { get; } = [];
    static ConcurrentDictionary<string, string> Histories { get; } = [];
    static ReaderWriterLockSlim ReaderWriterLock { get; } = new();
    static HashSet<string> IdentifyFlags { get; } = [];
}