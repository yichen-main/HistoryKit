namespace Eywa.Serve.Constructs.Grindstones.Quarterlies;
public interface ITimelineData
{
    void Write(in EnvironmentMeasure measure, in TimeseriesGroupInput input);
    void Write(in ManufactureMeasure measure, in TimeseriesGroupInput input);
    IAsyncEnumerable<(double value, DateTime timestamp)> ReadPerHourToDaysAsync(EnvironmentParamGroup input, int oldDayCount);
    IAsyncEnumerable<(double value, DateTime timestamp)> ReadPerHourAsync(EnvironmentParamGroup input, int oldHourCount);
    IAsyncEnumerable<(double value, DateTime timestamp)> ReadAsync(EnvironmentParam input, DateTime endTime, DateTime startTime);
    IAsyncEnumerable<(double value, DateTime timestamp)> ReadAsync(EnvironmentParamGroup input, DateTime endTime, DateTime startTime);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class TimelineData : ITimelineData
{
    public void Write(in EnvironmentMeasure measure, in TimeseriesGroupInput input)
    {
        InfluxHelper.Write(TimeseriesBucket.Environment, measure.ToString(), input);
    }
    public void Write(in ManufactureMeasure measure, in TimeseriesGroupInput input)
    {
        InfluxHelper.Write(TimeseriesBucket.Manufacture, measure.ToString(), input);
    }
    public async IAsyncEnumerable<(double value, DateTime timestamp)> ReadPerHourToDaysAsync(EnvironmentParamGroup input, int oldDayCount)
    {
        DateTime nowTime = DateTime.UtcNow;
        DateTime endTime = new(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, default, default);
        await foreach (var (value, timestamp) in InfluxHelper.QueryPerHourMedianAsync(TimeseriesBucket.Environment, new()
        {
            MeasurementName = input.Measure.ToString(),
            TagKey = input.TagKey,
            TagValue = input.TagValue,
            DataNo = input.DataNo,
            EndTime = endTime,
            StartTime = endTime.AddDays(-Math.Abs(oldDayCount)),
        }).ConfigureAwait(false)) yield return (value, timestamp ?? default);
    }
    public async IAsyncEnumerable<(double value, DateTime timestamp)> ReadPerHourAsync(EnvironmentParamGroup input, int oldHourCount)
    {
        DateTime nowTime = DateTime.UtcNow;
        DateTime endTime = new(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, default, default);
        await foreach (var (value, timestamp) in InfluxHelper.QueryPerHourMedianAsync(TimeseriesBucket.Environment, new()
        {
            MeasurementName = input.Measure.ToString(),
            TagKey = input.TagKey,
            TagValue = input.TagValue,
            DataNo = input.DataNo,
            EndTime = endTime,
            StartTime = endTime.AddHours(-Math.Abs(oldHourCount)),
        }).ConfigureAwait(false)) yield return (value, timestamp ?? default);
    }
    public async IAsyncEnumerable<(double value, DateTime timestamp)> ReadAsync(EnvironmentParam input, DateTime endTime, DateTime startTime)
    {
        await foreach (var (value, timestamp) in InfluxHelper.QueryAsync(TimeseriesBucket.Environment, new FluxData
        {
            MeasurementName = input.Measure.ToString(),
            DataNo = input.DataNo,
            EndTime = endTime,
            StartTime = startTime,
        }).ConfigureAwait(false)) yield return (value, timestamp ?? default);
    }
    public async IAsyncEnumerable<(double value, DateTime timestamp)> ReadAsync(EnvironmentParamGroup input, DateTime endTime, DateTime startTime)
    {
        await foreach (var (value, timestamp) in InfluxHelper.QueryAsync(TimeseriesBucket.Environment, new FluxDataGroup
        {
            MeasurementName = input.Measure.ToString(),
            TagKey = input.TagKey,
            TagValue = input.TagValue,
            DataNo = input.DataNo,
            EndTime = endTime,
            StartTime = startTime,
        }).ConfigureAwait(false)) yield return (value, timestamp ?? default);
    }
    public required IInfluxHelper InfluxHelper { get; init; }
}