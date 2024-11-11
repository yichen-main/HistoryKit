namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class ListFieldMemberVehicle : NodeEnlarge<ListFieldMemberVehicle, ListFieldMemberInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListFieldMemberInput req, CancellationToken ct)
    {
        IEnumerable<GetFieldMemberVehicle.Output> outputs = [];
        var usersAsync = CacheMediator.ListAllRegistrantsAsync(ct);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            List<GetFieldMemberVehicle.Output> results = [];
            var users = await usersAsync.ConfigureAwait(false);
            var domainMembers = await reader!.ReadAsync<SystemDomainMember>().ConfigureAwait(false);
            foreach (var domainMember in domainMembers)
            {
                var user = users.First(x => x.Id.IsMatch(domainMember.UserId));
                results.Add(new()
                {
                    Id = domainMember.Id,
                    Disable = domainMember.Disable,
                    FieldType = domainMember.FieldModule,
                    FieldName = DurableSetup.Link(domainMember.FieldModule),
                    AccessType = domainMember.RolePolicy,
                    AccessName = DurableSetup.Link(domainMember.RolePolicy),
                    UserId = user.Id,
                    Email = user.Email,
                    UserNo = user.UserNo,
                    UserName = user.UserName,
                    Creator = domainMember.Creator,
                    CreateTime = DurableSetup.LocalTime(domainMember.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = domainMember.Modifier,
                    ModifyTime = DurableSetup.LocalTime(domainMember.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
            outputs = req.UserId.IsDefault() ? results : results.Where(x => x.UserId.IsMatch(req.UserId));
            outputs = req.FieldType == default ? results : results.Where(x => x.FieldType == req.FieldType);
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<SystemDomainMember>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetFieldMemberVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
            (nameof(req.UserId), req.UserId),
            (nameof(req.FieldType), req.FieldType.ToString()),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}