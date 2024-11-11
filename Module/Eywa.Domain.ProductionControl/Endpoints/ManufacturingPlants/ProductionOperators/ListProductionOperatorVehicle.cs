namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class ListProductionOperatorVehicle : NodeEnlarge<ListProductionOperatorVehicle, ListProductionOperatorInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionOperatorInput req, CancellationToken ct)
    {
        List<GetProductionOperatorVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
            var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);
            var operators = await reader.ReadAsync<ProductionOperator>().ConfigureAwait(false);
            foreach (var @operator in operators)
            {
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
                    if (group is not null) outputs.Add(new()
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
                    });
                }
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionMachine>(),
                TableLayout.LetSelect<ProductionProcess>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetProductionOperatorVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}