namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class GetWorkingLocationVehicle : NodeEnlarge<GetWorkingLocationVehicle, GetWorkingLocationInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetWorkingLocationInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
            var stations = await reader!.ReadAsync<ProductionStation>().ConfigureAwait(false);
            var operators = await reader.ReadAsync<ProductionOperator>().ConfigureAwait(false);
            var groups = await reader.ReadAsync<ProductionGroup>().ConfigureAwait(false);
            var location = await reader.ReadSingleAsync<WorkshopLocation>().ConfigureAwait(false);

            var station = stations.FirstOrDefault(x => x.Id.IsMatch(location.StationId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

            var @operator = operators.FirstOrDefault(x => x.Id.IsMatch(location.OperatorId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceOperatorIdDoesNotExist));

            var group = groups.FirstOrDefault(x => x.Id.IsMatch(@operator.GroupId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceGroupIdDoesNotExist));

            var member = members.FirstOrDefault(x => x.Id.IsMatch(@operator.MemberId));
            if (member.Id == default) await PublishAsync(new DelWorkingLocationEvent
            {
                Id = location.Id,
            }, cancellation: ct).ConfigureAwait(false);
            else output = new()
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
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(),
                TableLayout.LetSelect<ProductionOperator>(),
                TableLayout.LetSelect<ProductionGroup>(),
                TableLayout.LetSelect<WorkshopLocation>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string StationId { get; init; }
        public required string StationNo { get; init; }
        public required string StationName { get; init; }
        public required string OperatorId { get; init; }
        public required string GroupId { get; init; }
        public required string GroupNo { get; init; }
        public required string GroupName { get; init; }
        public required string MemberId { get; init; }
        public required string MemberNo { get; init; }
        public required string MemberName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}