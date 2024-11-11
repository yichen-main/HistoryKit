namespace Eywa.Serve.Constructs.Adminiculars.Quarterlies;
internal interface IInfluxHelper
{
    void Write(in TimeseriesBucket bucket, in string measurementName, in TimeseriesGroupInput input);
    IAsyncEnumerable<(object value, DateTime? timestamp)> ReadAsync(FluxReader reader);
    IAsyncEnumerable<(double value, DateTime? timestamp)> QueryAsync(TimeseriesBucket bucket, FluxData input);
    IAsyncEnumerable<(double value, DateTime? timestamp)> QueryAsync(TimeseriesBucket bucket, FluxDataGroup input);
    IAsyncEnumerable<(double value, DateTime? timestamp)> QueryPerHourMedianAsync(TimeseriesBucket bucket, FluxDataGroup input);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class InfluxHelper : IInfluxHelper
{
    public void Write(in TimeseriesBucket bucket, in string measurementName, in TimeseriesGroupInput input)
    {
        if (!BaseCreator.EnableDatabase) return;
        using InfluxDBClient client = new(new InfluxDBClientOptions(BaseCreator.Influx.Url)
        {
            Org = BaseCreator.Influx.Org,
            Token = BaseCreator.Influx.Token,
            Bucket = bucket.ToString().ToSnakeCase(),
            LogLevel = InfluxDB.Client.Core.LogLevel.None,
        });
        using var writeApi = client.GetWriteApi();
        writeApi.WritePoint(PointData.Measurement(measurementName.ToSnakeCase()).Tag(input.TagName.ToSnakeCase(), input.TagValue)
        .Field(input.DataNo.ToSnakeCase(), input.DataValue).Timestamp(input.CreateTime.ToUniversalTime(), WritePrecision.Ns));
    }
    public async IAsyncEnumerable<(object value, DateTime? timestamp)> ReadAsync(FluxReader reader)
    {
        string[] conditions = [
            FluxSyntax.Range(reader.StartTime, reader.EndTime),
            FluxSyntax.FilterMeasurement(reader.MeasurementName.ToSnakeCase()),
        ];
        var merges = new string[conditions.Length + reader.Conditions.Length];
        Array.Copy(conditions, default, merges, default, conditions.Length);
        Array.Copy(reader.Conditions, default, merges, conditions.Length, reader.Conditions.Length);
        var query = FluxSyntax.From(reader.Bucket.ToString().ToSnakeCase(), merges);
        if (BaseCreator.EnableDatabase)
        {
            using InfluxDBClient client = new(BaseCreator.Influx.Url, BaseCreator.Influx.Token);
            var tables = await client.GetQueryApi().QueryAsync(query, BaseCreator.Influx.Org).ConfigureAwait(false);
            foreach (var record in tables.SelectMany(x => x.Records))
            {
                yield return (record.GetValue(), record.GetTimeInDateTime());
            }
        }
    }
    public async IAsyncEnumerable<(double value, DateTime? timestamp)> QueryAsync(TimeseriesBucket bucket, FluxData input)
    {
        await foreach (var (value, timestamp) in ReadAsync(new()
        {
            Bucket = bucket,
            MeasurementName = input.MeasurementName,
            EndTime = input.EndTime,
            StartTime = input.StartTime,
            Conditions = [
                FluxSyntax.FilterField(input.DataNo.ToSnakeCase()),
            ],
        }).ConfigureAwait(false))
        {
            if (value.TryDouble(out double result)) yield return (result, timestamp);
        }
    }
    public async IAsyncEnumerable<(double value, DateTime? timestamp)> QueryAsync(TimeseriesBucket bucket, FluxDataGroup input)
    {
        await foreach (var (value, timestamp) in ReadAsync(new()
        {
            Bucket = bucket,
            MeasurementName = input.MeasurementName,
            EndTime = input.EndTime,
            StartTime = input.StartTime,
            Conditions = [
                FluxSyntax.Filter(input.TagKey.ToSnakeCase(),input.TagValue),
                FluxSyntax.FilterField(input.DataNo.ToSnakeCase()),
            ],
        }).ConfigureAwait(false))
        {
            if (value.TryDouble(out double result)) yield return (result, timestamp);
        }
    }
    public async IAsyncEnumerable<(double value, DateTime? timestamp)> QueryPerHourMedianAsync(TimeseriesBucket bucket, FluxDataGroup input)
    {
        await foreach (var (value, timestamp) in ReadAsync(new()
        {
            Bucket = bucket,
            MeasurementName = input.MeasurementName,
            EndTime = input.EndTime,
            StartTime = input.StartTime,
            Conditions = [
                FluxSyntax.Filter(input.TagKey.ToSnakeCase(),input.TagValue),
                FluxSyntax.FilterField(input.DataNo.ToSnakeCase()),
                FluxSyntax.AggregateWindowPerHourMedian(),
            ],
        }).ConfigureAwait(false))
        {
            if (value.TryDouble(out double result)) yield return (result, timestamp);
        }
    }
    public required IFluxSyntax FluxSyntax { get; init; }
    public required IBaseCreator BaseCreator { get; init; }
}