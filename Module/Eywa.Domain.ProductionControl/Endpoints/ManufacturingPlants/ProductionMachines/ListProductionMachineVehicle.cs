namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class ListProductionMachineVehicle : NodeEnlarge<ListProductionMachineVehicle, ListProductionMachineInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionMachineInput req, CancellationToken ct)
    {
        List<GetProductionMachineVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);
            var machines = await reader!.ReadAsync<ProductionMachine>().ConfigureAwait(false);
            var processes = await reader.ReadAsync<ProductionProcess>().ConfigureAwait(false);
            foreach (var machine in machines)
            {
                var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(machine.EquipmentId));
                if (equipment.Id == default)
                {
                    var machineChilds = ProductionMachine.GetEquipmentChildDeleter(connection, machine.EquipmentId);
                    await foreach (var machineChild in machineChilds.ConfigureAwait(false).WithCancellation(ct))
                    {
                        await connection.ExecuteAsync(machineChild, transaction).ConfigureAwait(false);
                    }
                }
                else
                {
                    var process = processes.FirstOrDefault(x => x.Id.IsMatch(machine.ProcessId));
                    if (process is not null) outputs.Add(new()
                    {
                        Id = machine.Id,
                        EquipmentId = equipment.Id,
                        EquipmentNo = equipment.EquipmentNo,
                        EquipmentName = equipment.EquipmentName,
                        ProcessId = process.Id,
                        ProcessNo = process.ProcessNo,
                        ProcessName = process.ProcessName,
                        Creator = machine.Creator,
                        CreateTime = DurableSetup.LocalTime(machine.CreateTime, req.TimeZone, req.TimeFormat),
                        Modifier = machine.Modifier,
                        ModifyTime = DurableSetup.LocalTime(machine.ModifyTime, req.TimeZone, req.TimeFormat),
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
        Pagination(new PageContents<GetProductionMachineVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}