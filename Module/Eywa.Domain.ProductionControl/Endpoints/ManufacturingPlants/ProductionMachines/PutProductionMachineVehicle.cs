namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class PutProductionMachineVehicle() : NodeEnlarge<PutProductionMachineVehicle, PutProductionMachineInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionMachineInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);
                var processes = await reader!.ReadAsync<ProductionProcess>().ConfigureAwait(false);

                var machine = await reader.ReadFirstOrDefaultAsync<ProductionMachine>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceMachineIdDoesNotExist));

                var process = processes.FirstOrDefault(x => x.Id.IsMatch(req.Body.ProcessId))
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
                else await connection.ExecuteAsync(DurableSetup.DelimitUpdate(machine.Id, new ProductionMachine
                {
                    EquipmentId = equipment.Id,
                    ProcessId = process.Id,
                    Modifier = GetUserName(),
                    Creator = machine.Creator,
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
                TableLayout.LetSelect<ProductionProcess>(),
                TableLayout.LetSelect<ProductionMachine>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}