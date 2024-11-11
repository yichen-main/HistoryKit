namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class ListFieldSupervisorVehicle : NodeEnlarge<ListFieldSupervisorVehicle, ListFieldSupervisorInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListFieldSupervisorInput req, CancellationToken ct)
    {
        IEnumerable<GetFieldSupervisorVehicle.Output> outputs = [];
        await VerifyAsync(ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            List<GetFieldSupervisorVehicle.Output> results = [];
            var users = await reader!.ReadAsync<UserRegistration>().ConfigureAwait(false);
            var modules = await reader.ReadAsync<SystemDomainModule>().ConfigureAwait(false);
            foreach (var module in modules)
            {
                var user = users.First(x => x.Id.IsMatch(module.UserId));
                results.Add(new()
                {
                    Id = module.Id,
                    Disable = user.Disable,
                    FieldType = module.FieldType,
                    FieldName = DurableSetup.Link(module.FieldType),
                    UserId = user.Id,
                    Email = user.Email,
                    UserNo = user.UserNo,
                    UserName = user.UserName,
                    Creator = module.Creator,
                    CreateTime = DurableSetup.LocalTime(module.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = module.Modifier,
                    ModifyTime = DurableSetup.LocalTime(module.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
            outputs = req.UserId.IsDefault() ? results : results.Where(x => x.UserId.IsMatch(req.UserId));
            outputs = req.FieldType == default ? results : results.Where(x => x.FieldType == req.FieldType);
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(),
                TableLayout.LetSelect<SystemDomainModule>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetFieldSupervisorVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
            (nameof(req.UserId), req.UserId),
            (nameof(req.FieldType), req.FieldType.ToString()),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}