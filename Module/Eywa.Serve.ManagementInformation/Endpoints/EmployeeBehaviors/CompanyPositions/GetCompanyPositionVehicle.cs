namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.CompanyPositions;
internal sealed class GetCompanyPositionVehicle : NodeEnlarge<GetCompanyPositionVehicle, GetCompanyPositionInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetCompanyPositionInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.HumanResources, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var position = await reader!.ReadFirstAsync<EmployeePosition>().ConfigureAwait(false);
            output = new()
            {
                Id = position.Id,
                PositionNo = position.PositionNo,
                PositionName = position.PositionName,
                Creator = position.Creator,
                CreateTime = DurableSetup.LocalTime(position.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = position.Modifier,
                ModifyTime = DurableSetup.LocalTime(position.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<EmployeePosition>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string PositionNo { get; init; }
        public required string PositionName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}