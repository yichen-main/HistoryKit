namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface IBaseCreator
{
    void Initialize();
    void SetEnableDatabase();
    string Culture(in string key);
    ValueTask CreateBucketAsync();
    Task CreateNpgsqlAsync(Type type, CancellationToken ct);
    void Configure(in string? connectionString, in ServiceTerritory.TextInfluxDB settings);
    Task SendAsync(string address, string subject, string text, TextFormat format = TextFormat.Html);
    bool EnableDatabase { get; }
    string? ConnectionString { get; }
    (string Url, string Token, string Org) Influx { get; }
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class BaseCreator : IBaseCreator
{
    ConcurrentBag<SmtpClient>? _smtpPool;
    public void Initialize()
    {
        if (_smtpPool is null)
        {
            _smtpPool = [];
            for (int i = default; i < HostInformation.Value.SMTP.PoolSize; i++) _smtpPool.Add(SmtpClient);
        }
    }
    public void SetEnableDatabase() => EnableDatabase = true;
    public string Culture(in string key) => Localizer[key];
    public async ValueTask CreateBucketAsync()
    {
        using InfluxDBClient client = new(Influx.Url, Influx.Token);
        foreach (TimeseriesBucket item in Enum.GetValues(typeof(TimeseriesBucket)))
        {
            var name = item.ToString().ToSnakeCase();
            var bucket = await client.GetBucketsApi().FindBucketByNameAsync(name).ConfigureAwait(false);
            if (bucket is null)
            {
                BucketRetentionRules retention = new(BucketRetentionRules.TypeEnum.Expire, item.GetHashCode());
                var orgs = await client.GetOrganizationsApi().FindOrganizationsAsync(org: Influx.Org).ConfigureAwait(false);
                await client.GetBucketsApi().CreateBucketAsync(name, retention, orgs[default].Id).ConfigureAwait(false);
            }
        }
    }
    public async Task CreateNpgsqlAsync(Type type, CancellationToken ct)
    {
        List<string> columns = [];
        List<string> indexes = [];
        var properties = type.GetProperties();
        for (int i = default; i < properties.Length; i++)
        {
            var property = properties[i];
            var columnName = property.Name.ToSnakeCase();
            var rowInfo = property.GetCustomAttribute<RowInfoAttribute>();
            if (rowInfo is { UniqueIndex: true }) indexes.Add(TableLayout.LetUniqueIndex(type.Name.ToSnakeCase(), columnName));
            switch (property.Name)
            {
                case nameof(NpgsqlBase.Id):
                    columns.Add($"{columnName} CHARACTER(18) PRIMARY KEY");
                    break;

                default:
                    if (rowInfo is { ForeignKey: true })
                    {
                        columns.Add($"{columnName} CHARACTER(18) NOT NULL");
                    }
                    else columns.Add($"{columnName} {property.PropertyType switch
                    {
                        var x when x.IsEnum => "SMALLINT",
                        var x when x.Equals(typeof(Guid)) => "UUID",
                        var x when x.Equals(typeof(bool)) => "BOOLEAN",
                        var x when x.Equals(typeof(short)) => "SMALLINT",
                        var x when x.Equals(typeof(int)) => "INTEGER",
                        var x when x.Equals(typeof(long)) => "BIGINT",
                        var x when x.Equals(typeof(float)) => "REAL",
                        var x when x.Equals(typeof(double)) => "DOUBLE PRECISION",
                        var x when x.Equals(typeof(decimal)) => "MONEY",
                        var x when x.Equals(typeof(string[])) => "TEXT[]",
                        var x when x.Equals(typeof(TimeSpan)) => "TIME",
                        var x when x.Equals(typeof(DateTime)) => "TIMESTAMP WITHOUT TIME ZONE",
                        _ => "VARCHAR",
                    }} NOT NULL");
                    break;
            }
        }
        NpgsqlConnection connection = new(ConnectionString);
        await using (connection.ConfigureAwait(false))
        {
            await connection.OpenAsync(ct).ConfigureAwait(false);
            await connection.ExecuteAsync(TableLayout.LetCreate(type, columns)).ConfigureAwait(false);
            foreach (var index in indexes) await connection.ExecuteAsync(index).ConfigureAwait(false);
        }
    }
    public void Configure(in string? connectionString, in ServiceTerritory.TextInfluxDB settings)
    {
        ConnectionString = connectionString;
        Influx = (settings.Endpoint, settings.Token, settings.Organization);
    }
    public async Task SendAsync(string address, string subject, string text, TextFormat format = TextFormat.Html)
    {
        var client = GetClient();
        try
        {
            using MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(HostInformation.Value.SMTP.Sender));
            message.To.Add(MailboxAddress.Parse(address));
            message.Subject = subject;
            message.Body = new TextPart(format)
            {
                Text = text,
            };
            await client.SendAsync(message).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (client.IsConnected && _smtpPool is not null) _smtpPool.Add(client);
            else client.Dispose();
        }
        SmtpClient GetClient() => _smtpPool switch
        {
            null => throw new Exception("SmtpClient pool not initialized."),
            _ => _smtpPool.TryTake(out var client) ? client : SmtpClient,
        };
    }
    SmtpClient SmtpClient
    {
        get
        {
            SmtpClient client = new();
            var smtp = HostInformation.Value.SMTP;
            client.Connect(smtp.Host, smtp.Port, SecureSocketOptions.StartTls);
            client.Authenticate(smtp.Sender, smtp.Password);
            return client;
        }
    }
    public bool EnableDatabase { get; private set; }
    public string? ConnectionString { get; private set; }
    public (string Url, string Token, string Org) Influx { get; private set; }
    public required IStringLocalizer<BaseCreator> Localizer { get; init; }
    public required IOptions<HostInformation> HostInformation { get; init; }
}