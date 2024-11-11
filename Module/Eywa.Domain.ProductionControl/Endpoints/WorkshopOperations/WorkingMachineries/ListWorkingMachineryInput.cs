namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class ListWorkingMachineryInput : NodeHeader
{
    public string? StationId { get; init; }
    public string? MachineId { get; init; }
    public string? EquipmentId { get; init; }
    public string? ProcessId { get; init; }
    public sealed class Validator : AbstractValidator<ListWorkingMachineryInput>;
}