namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.CompanyPositions;
internal sealed class AddCompanyPositionVehicle() : NodeEnlarge<AddCompanyPositionVehicle, AddCompanyPositionInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddCompanyPositionInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await VerifyAsync(ct).ConfigureAwait(false);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new EmployeePosition
                {
                    Id = id,
                    PositionNo = req.Body.PositionNo,
                    PositionName = req.Body.PositionName,
                    Creator = userName,
                    Modifier = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (EmployeePosition.PositionNoIndex, DurableSetup.Link(HumanResourcesFlag.HumanPositionNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetCompanyPositionVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}