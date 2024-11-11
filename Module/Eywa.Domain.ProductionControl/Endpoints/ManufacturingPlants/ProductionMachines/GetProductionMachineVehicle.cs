namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class GetProductionMachineVehicle : NodeEnlarge<GetProductionMachineVehicle, GetProductionMachineInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionMachineInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);
            var processes = await reader!.ReadAsync<ProductionProcess>().ConfigureAwait(false);
            var machine = await reader!.ReadSingleAsync<ProductionMachine>().ConfigureAwait(false);
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
                if (process is not null) output = new()
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
                };
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionProcess>(),
                TableLayout.LetSelect<ProductionMachine>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
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