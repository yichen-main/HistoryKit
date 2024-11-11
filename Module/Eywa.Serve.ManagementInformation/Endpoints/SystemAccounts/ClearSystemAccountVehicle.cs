using File = System.IO.File;

namespace Eywa.Serve.ManagementInformation.Endpoints.SystemAccounts;
internal sealed class ClearSystemAccountVehicle : NodeEnlarge<ClearSystemAccountVehicle, ClearSystemAccountInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ClearSystemAccountInput req, CancellationToken ct)
    {
        await VerifyAsync(ct).ConfigureAwait(false);
        if (File.Exists(CiphertextPolicy.RootFilePath)) File.Delete(CiphertextPolicy.RootFilePath);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}