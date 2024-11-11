namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class AddWorkingMachineryVehicle() : NodeEnlarge<AddWorkingMachineryVehicle, AddWorkingMachineryInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddWorkingMachineryInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var equipments = await CacheMediator.ListFacilityEquipmentAsync(ct).ConfigureAwait(false);

                var station = await reader!.ReadFirstOrDefaultAsync<ProductionStation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

                var machine = await reader.ReadFirstOrDefaultAsync<ProductionMachine>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceMachineIdDoesNotExist));

                var equipment = equipments.FirstOrDefault(x => x.Id.IsMatch(machine.EquipmentId));
                if (equipment.Id == default) throw new Exception(DurableSetup.Link(FacilityManagementFlag.IndustrialEquipmentIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new WorkshopMachinery
                {
                    Id = id,
                    StationId = req.Body.StationId,
                    MachineId = machine.Id,
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
                TableLayout.LetSelect<ProductionStation>(req.Body.StationId),
                TableLayout.LetSelect<ProductionMachine>(req.Body.MachineId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetWorkingMachineryVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}