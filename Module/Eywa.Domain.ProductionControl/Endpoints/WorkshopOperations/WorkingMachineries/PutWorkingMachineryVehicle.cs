namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class PutWorkingMachineryVehicle() : NodeEnlarge<PutWorkingMachineryVehicle, PutWorkingMachineryInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutWorkingMachineryInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);

                var machinery = await reader!.ReadFirstOrDefaultAsync<WorkshopMachinery>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.WorkingLocationIdDoesNotExist));

                var station = await reader.ReadFirstOrDefaultAsync<ProductionStation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

                var machine = await reader.ReadFirstOrDefaultAsync<ProductionMachine>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceMachineIdDoesNotExist));

                var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(machine.EquipmentId));
                if (equipment.Id == default) throw new Exception(DurableSetup.Link(FacilityManagementFlag.IndustrialEquipmentIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitUpdate(machinery.Id, new WorkshopMachinery
                {
                    StationId = station.Id,
                    MachineId = machine.Id,
                    Modifier = GetUserName(),
                    Creator = machinery.Creator,
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
                TableLayout.LetSelect<WorkshopMachinery>(req.Body.Id),
                TableLayout.LetSelect<ProductionStation>(req.Body.StationId),
                TableLayout.LetSelect<ProductionMachine>(req.Body.MachineId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}