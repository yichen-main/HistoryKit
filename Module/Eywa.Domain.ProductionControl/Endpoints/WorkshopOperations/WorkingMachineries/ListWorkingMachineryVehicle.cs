namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class ListWorkingMachineryVehicle : NodeEnlarge<ListWorkingMachineryVehicle, ListWorkingMachineryInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListWorkingMachineryInput req, CancellationToken ct)
    {
        IEnumerable<GetWorkingMachineryVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            List<GetWorkingMachineryVehicle.Output> results = [];
            var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);
            var stations = await reader!.ReadAsync<ProductionStation>().ConfigureAwait(false);
            var machines = await reader.ReadAsync<ProductionMachine>().ConfigureAwait(false);
            var processes = await reader.ReadAsync<ProductionProcess>().ConfigureAwait(false);
            var machineries = await reader.ReadAsync<WorkshopMachinery>().ConfigureAwait(false);
            foreach (var machinery in machineries)
            {
                var station = stations.FirstOrDefault(x => x.Id.IsMatch(machinery.StationId));
                if (station is null) continue;

                var machine = machines.FirstOrDefault(x => x.Id.IsMatch(machinery.MachineId));
                if (machine is null) continue;

                var process = processes.FirstOrDefault(x => x.Id.IsMatch(machine.ProcessId));
                if (process is null) continue;

                var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(machine.EquipmentId));
                if (equipment.Id == default) await PublishAsync(new DelWorkingMachineryEvent
                {
                    Id = machinery.Id,
                }, cancellation: ct).ConfigureAwait(false);
                else results.Add(new()
                {
                    Id = machinery.Id,
                    StationId = station.Id,
                    StationNo = station.StationNo,
                    StationName = station.StationName,
                    MachineId = machine.Id,
                    EquipmentId = equipment.Id,
                    EquipmentNo = equipment.EquipmentNo,
                    EquipmentName = equipment.EquipmentName,
                    ProcessId = process.Id,
                    ProcessNo = process.ProcessNo,
                    ProcessName = process.ProcessName,
                    Creator = machinery.Creator,
                    CreateTime = DurableSetup.LocalTime(machinery.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = machinery.Modifier,
                    ModifyTime = DurableSetup.LocalTime(machinery.ModifyTime, req.TimeZone, req.TimeFormat),
                });
                outputs = req.StationId.IsDefault() ? results : results.Where(x => x.StationId.IsMatch(req.StationId));
                outputs = req.MachineId.IsDefault() ? results : results.Where(x => x.MachineId.IsMatch(req.MachineId));
                outputs = req.EquipmentId.IsDefault() ? results : results.Where(x => x.EquipmentId.IsMatch(req.EquipmentId));
                outputs = req.ProcessId.IsDefault() ? results : results.Where(x => x.ProcessId.IsMatch(req.ProcessId));
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(),
                TableLayout.LetSelect<ProductionMachine>(),
                TableLayout.LetSelect<ProductionProcess>(),
                TableLayout.LetSelect<WorkshopMachinery>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetWorkingMachineryVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
            (nameof(req.StationId), req.StationId),
            (nameof(req.MachineId), req.MachineId),
            (nameof(req.EquipmentId), req.EquipmentId),
            (nameof(req.ProcessId), req.ProcessId),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}