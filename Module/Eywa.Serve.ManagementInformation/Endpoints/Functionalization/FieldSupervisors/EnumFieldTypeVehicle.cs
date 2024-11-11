namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class EnumFieldTypeVehicle : NodeEnlarge<EnumFieldTypeVehicle, EnumFieldTypeInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(EnumFieldTypeInput req, CancellationToken ct)
    {
        List<EnumCistern> results = [];
        await VerifyAsync(ct).ConfigureAwait(false);
        foreach (var module in BeforeExpand.GetFieldModules()) results.Add(new()
        {
            TypeNo = module.GetHashCode(),
            TypeName = DurableSetup.Link(module),
        });
        await SendAsync(results, cancellation: ct).ConfigureAwait(false);
    }
}