namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class AddProductionMachineVehicle() : NodeEnlarge<AddProductionMachineVehicle, AddProductionMachineInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionMachineInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);

                var process = await reader!.ReadFirstOrDefaultAsync<ProductionProcess>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceProcessIdDoesNotExist));

                var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(req.Body.EquipmentId));
                if (equipment.Id == default)
                {
                    var machineChilds = ProductionMachine.GetEquipmentChildDeleter(connection, req.Body.EquipmentId);
                    await foreach (var machineChild in machineChilds.ConfigureAwait(false).WithCancellation(ct))
                    {
                        await connection.ExecuteAsync(machineChild, transaction).ConfigureAwait(false);
                    }
                }
                else await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionMachine
                {
                    Id = id,
                    EquipmentId = equipment.Id,
                    ProcessId = process.Id,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException();
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionProcess>(req.Body.ProcessId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionMachineVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}