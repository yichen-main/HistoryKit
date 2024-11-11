namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class ListWorkingLocationVehicle : NodeEnlarge<ListWorkingLocationVehicle, ListWorkingLocationInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListWorkingLocationInput req, CancellationToken ct)
    {
        List<GetWorkingLocationVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
            var stations = await reader!.ReadAsync<ProductionStation>().ConfigureAwait(false);
            var operators = await reader.ReadAsync<ProductionOperator>().ConfigureAwait(false);
            var groups = await reader.ReadAsync<ProductionGroup>().ConfigureAwait(false);
            var locations = await reader.ReadAsync<WorkshopLocation>().ConfigureAwait(false);
            foreach (var location in locations)
            {
                var station = stations.FirstOrDefault(x => x.Id.IsMatch(location.StationId));
                if (station is null) continue;

                var @operator = operators.FirstOrDefault(x => x.Id.IsMatch(location.OperatorId));
                if (@operator is null) continue;

                var group = groups.FirstOrDefault(x => x.Id.IsMatch(@operator.GroupId));
                if (group is null) continue;

                var member = members.FirstOrDefault(x => x.Id.IsMatch(@operator.MemberId));
                if (member.Id == default) await PublishAsync(new DelWorkingLocationEvent
                {
                    Id = location.Id,
                }, cancellation: ct).ConfigureAwait(false);
                else outputs.Add(new()
                {
                    Id = location.Id,
                    StationId = station.Id,
                    StationNo = station.StationNo,
                    StationName = station.StationName,
                    OperatorId = @operator.Id,
                    GroupId = group.Id,
                    GroupNo = group.GroupNo,
                    GroupName = group.GroupName,
                    MemberId = member.Id,
                    MemberNo = member.UserNo,
                    MemberName = member.UserName,
                    Creator = location.Creator,
                    CreateTime = DurableSetup.LocalTime(location.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = location.Modifier,
                    ModifyTime = DurableSetup.LocalTime(location.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(),
                TableLayout.LetSelect<ProductionOperator>(),
                TableLayout.LetSelect<ProductionGroup>(),
                TableLayout.LetSelect<WorkshopLocation>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetWorkingLocationVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}