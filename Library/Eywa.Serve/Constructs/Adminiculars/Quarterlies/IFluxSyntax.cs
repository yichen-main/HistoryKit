namespace Eywa.Serve.Constructs.Adminiculars.Quarterlies;
internal interface IFluxSyntax
{
    string From(in string bucket, in string[] fields);
    string Range(in TimeseriesRange range);
    string Range(in DateTime startTime, in DateTime endTime);
    string Filter(in string key, in string name);
    string FilterMeasurement(in string name);
    string FilterField(in string name);
    string AggregateWindowPerHourMedian(in bool createEmpty = true);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class FluxSyntax : IFluxSyntax
{
    public string From(in string bucket, in string[] fields)
    {
        StringBuilder builder = new($"""from(bucket: "{bucket}")""");
        for (int i = default; i < fields.Length; i++) builder.AppendLine(fields[i]);
        return builder.ToString();
    }
    public string Range(in TimeseriesRange range) => $"|> range(start: {range.GetDesc()})";
    public string Range(in DateTime startTime, in DateTime endTime)
    {
        return $"|> range(start: {To(startTime)}, stop: {To(endTime)})";
        static string To(in DateTime time) => time.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
    }
    public string Filter(in string key, in string name) => $"""|> filter(fn: (r) => (r["{key}"] == "{name}"))""";
    public string FilterMeasurement(in string name) => Filter("_measurement", name);
    public string FilterField(in string name) => Filter("_field", name);
    public string AggregateWindowPerHourMedian(in bool createEmpty = true)
    {
        var emptyTag = createEmpty is true ? "true" : "false";
        return $"|> aggregateWindow(every: 1h, fn: median, createEmpty: {emptyTag})";
    }
}