namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface IDurableSetup
{
    string Link(in RolePolicy type);
    string Link(in FieldModule type);
    string Link(in LoginStatus type);
    string Link(in AccountAccessFlag type);
    string Link(in EnterpriseIntegrationFlag type);
    string Link(in FacilityManagementFlag type);
    string Link(in HumanResourcesFlag type);
    string Link(in ProductionControlFlag type);
    DateTime ParseDateTimeFormat(in string dateTime, in string format);
    string LocalTime(in DateTime dateTime, in int timeZone, in string timeFormat);
    CommandDefinition DelimitInsert<T>(in T entity, in CancellationToken ct) where T : NpgsqlBase;
    CommandDefinition DelimitInsert<T>(in T entity, in NpgsqlTransaction transaction, in CancellationToken ct) where T : NpgsqlBase;
    CommandDefinition DelimitUpdate<T>(in string id, in T entity, in CancellationToken ct, in string[]? fields = null) where T : NpgsqlBase;
    Task ExecuteAsync(Func<NpgsqlConnection, SqlMapper.GridReader?, Task> options, NpgsqlProvider provider);
    Task TransactionAsync(Func<NpgsqlConnection, NpgsqlTransaction, SqlMapper.GridReader?, Task> options, NpgsqlProvider provider);
    Task SendEmailAsync(in string address, in string subject, in string text);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class DurableSetup : IDurableSetup
{
    public string Link(in RolePolicy type) => BaseCreator.Culture(type.ToString());
    public string Link(in FieldModule type) => BaseCreator.Culture(type.ToString());
    public string Link(in LoginStatus type) => BaseCreator.Culture(type.ToString());
    public string Link(in AccountAccessFlag type) => BaseCreator.Culture(type.ToString());
    public string Link(in EnterpriseIntegrationFlag type) => BaseCreator.Culture(type.ToString());
    public string Link(in FacilityManagementFlag type) => BaseCreator.Culture(type.ToString());
    public string Link(in HumanResourcesFlag type) => BaseCreator.Culture(type.ToString());
    public string Link(in ProductionControlFlag type) => BaseCreator.Culture(type.ToString());
    public DateTime ParseDateTimeFormat(in string dateTime, in string format)
    {
        try
        {
            return DateTime.ParseExact(dateTime, format, CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            return default;
        }
    }
    public string LocalTime(in DateTime dateTime, in int timeZone, in string timeFormat)
    {
        return dateTime.AddHours(timeZone).ToString(timeFormat, CultureInfo.InvariantCulture);
    }
    public CommandDefinition DelimitInsert<T>(in T entity, in CancellationToken ct) where T : NpgsqlBase
    {
        var (sql, param) = TableLayout.LetInsert(entity);
        return new(commandText: sql, parameters: param, commandType: CommandType.Text, cancellationToken: ct);
    }
    public CommandDefinition DelimitInsert<T>(in T entity, in NpgsqlTransaction transaction, in CancellationToken ct) where T : NpgsqlBase
    {
        var (sql, param) = TableLayout.LetInsert(entity);
        return new(commandText: sql, parameters: param, transaction: transaction, commandType: CommandType.Text, cancellationToken: ct);
    }
    public CommandDefinition DelimitUpdate<T>(in string id, in T entity, in CancellationToken ct, in string[]? fields = null) where T : NpgsqlBase
    {
        List<string> filters = [nameof(NpgsqlBase.Id), nameof(NpgsqlBase.CreateTime)];
        if (fields is not null) foreach (var field in fields) filters.Add(field);
        var (sql, param) = TableLayout.LetUpdate(id, entity, nameof(NpgsqlBase.Id), filters);
        return new(commandText: sql, parameters: param, commandType: CommandType.Text, cancellationToken: ct);
    }
    public async Task ExecuteAsync(Func<NpgsqlConnection, SqlMapper.GridReader?, Task> options, NpgsqlProvider provider)
    {
        if (!BaseCreator.EnableDatabase) return;
        ArgumentNullException.ThrowIfNull(options);
        NpgsqlConnection connection = new(BaseCreator.ConnectionString);
        await using (connection.ConfigureAwait(false))
        {
            SqlMapper.GridReader? reader = default;
            await connection.OpenAsync(provider.CancellationToken).ConfigureAwait(false);
            var sql = TableLayout.JoinQueries(provider.QueryFields);
            if (sql is not null) reader = await connection.QueryMultipleAsync(sql).ConfigureAwait(false);
            await options(connection, reader).ConfigureAwait(false);
        }
    }
    public async Task TransactionAsync(Func<NpgsqlConnection, NpgsqlTransaction, SqlMapper.GridReader?, Task> options, NpgsqlProvider provider)
    {
        if (!BaseCreator.EnableDatabase) return;
        ArgumentNullException.ThrowIfNull(options);
        NpgsqlConnection connection = new(BaseCreator.ConnectionString);
        await using (connection.ConfigureAwait(false))
        {
            SqlMapper.GridReader? reader = default;
            await connection.OpenAsync(provider.CancellationToken).ConfigureAwait(false);
            var transaction = await connection.BeginTransactionAsync(provider.CancellationToken).ConfigureAwait(false);
            var sql = TableLayout.JoinQueries(provider.QueryFields);
            try
            {
                if (sql is not null) reader = await connection.QueryMultipleAsync(sql).ConfigureAwait(false);
                await options(connection, transaction, reader).ConfigureAwait(false);
                await transaction.CommitAsync(provider.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(provider.CancellationToken).ConfigureAwait(false);
                throw;
            }
            finally
            {
                await transaction.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
    public Task SendEmailAsync(in string address, in string subject, in string text) => BaseCreator.SendAsync(address, subject, text);
    public required IBaseCreator BaseCreator { get; init; }
}