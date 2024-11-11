namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.ActivityRecords;
internal sealed class ListActivityRecordVehicle : NodeEnlarge<ListActivityRecordVehicle, ListActivityRecordInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListActivityRecordInput req, CancellationToken ct)
    {
        IEnumerable<Output> outputs = [];
        await VerifyAsync(ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            List<Output> results = [];
            var users = await reader!.ReadAsync<UserRegistration>().ConfigureAwait(false);
            var records = await reader.ReadAsync<UserActivityRecord>().ConfigureAwait(false);
            foreach (var record in records)
            {
                var user = users.First(x => x.Id.IsMatch(record.UserId));
                results.Add(new()
                {
                    Id = record.Id,
                    StatusName = DurableSetup.Link(record.LoginStatus),
                    UserId = user.Id,
                    Email = user.Email,
                    UserNo = user.UserNo,
                    UserName = user.UserName,
                    CreateTime = DurableSetup.LocalTime(record.CreateTime, req.TimeZone, req.TimeFormat),
                });
            }
            outputs = req.UserId.IsDefault() ? results : results.Where(x => x.UserId.IsMatch(req.UserId));
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(),
                TableLayout.LetSelect<UserActivityRecord>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
            (nameof(req.UserId), req.UserId),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
    public readonly struct Output
    {
        public required string Id { get; init; }
        public required string StatusName { get; init; }
        public required string UserId { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
        public required string CreateTime { get; init; }
    }
}