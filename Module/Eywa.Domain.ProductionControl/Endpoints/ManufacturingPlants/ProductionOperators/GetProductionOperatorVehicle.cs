namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class GetProductionOperatorVehicle : NodeEnlarge<GetProductionOperatorVehicle, GetProductionOperatorInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionOperatorInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
            var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);
            var @operator = await reader.ReadSingleAsync<ProductionOperator>().ConfigureAwait(false);
            var member = members.FirstOrDefault(x => x.Id.IsMatch(@operator.MemberId));
            if (member.Id == default)
            {
                var memberChilds = ProductionOperator.GetMemberChildDeleter(connection, @operator.MemberId);
                await foreach (var memberChild in memberChilds.ConfigureAwait(false).WithCancellation(ct))
                {
                    await connection.ExecuteAsync(memberChild, transaction).ConfigureAwait(false);
                }
            }
            else
            {
                var group = groups.FirstOrDefault(x => x.Id.IsMatch(@operator.GroupId));
                if (group is not null) output = new()
                {
                    Id = @operator.Id,
                    GroupId = group.Id,
                    GroupNo = group.GroupNo,
                    GroupName = group.GroupName,
                    MemberId = member.Id,
                    MemberNo = member.UserNo,
                    MemberName = member.UserName,
                    Creator = @operator.Creator,
                    CreateTime = DurableSetup.LocalTime(@operator.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = @operator.Modifier,
                    ModifyTime = DurableSetup.LocalTime(@operator.ModifyTime, req.TimeZone, req.TimeFormat),
                };
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionGroup>(),
                TableLayout.LetSelect<ProductionOperator>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
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