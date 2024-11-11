namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class GetWorkingMachineryVehicle : NodeEnlarge<GetWorkingMachineryVehicle, GetWorkingMachineryInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetWorkingMachineryInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);
            var stations = await reader!.ReadAsync<ProductionStation>().ConfigureAwait(false);
            var machines = await reader.ReadAsync<ProductionMachine>().ConfigureAwait(false);
            var processes = await reader.ReadAsync<ProductionProcess>().ConfigureAwait(false);
            var machinery = await reader.ReadSingleAsync<WorkshopMachinery>().ConfigureAwait(false);

            var station = stations.FirstOrDefault(x => x.Id.IsMatch(machinery.StationId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

            var machine = machines.FirstOrDefault(x => x.Id.IsMatch(machinery.MachineId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceMachineIdDoesNotExist));

            var process = processes.FirstOrDefault(x => x.Id.IsMatch(machine.ProcessId))
            ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceProcessIdDoesNotExist));

            var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(machine.EquipmentId));
            if (equipment.Id == default)
            {
                var machineChilds = ProductionMachine.GetEquipmentChildDeleter(connection, machine.EquipmentId);
                await foreach (var machineChild in machineChilds.ConfigureAwait(false).WithCancellation(ct))
                {
                    await connection.ExecuteAsync(machineChild, transaction).ConfigureAwait(false);
                }
            }
            else output = new()
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
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(),
                TableLayout.LetSelect<ProductionMachine>(),
                TableLayout.LetSelect<ProductionProcess>(),
                TableLayout.LetSelect<WorkshopMachinery>(req.Id),
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
        public required string MachineId { get; init; }
        public required string EquipmentId { get; init; }
        public required string EquipmentNo { get; init; }
        public required string EquipmentName { get; init; }
        public required string ProcessId { get; init; }
        public required string ProcessNo { get; init; }
        public required string ProcessName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}