namespace Eywa.Serve.ManagementInformation.Endpoints.SystemAccounts;
internal sealed class ListSystemAccountVehicle : NodeEnlarge<ListSystemAccountVehicle, ListSystemAccountInput>
{
    public override void Configure()
    {
        AllowAnonymous();
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListSystemAccountInput req, CancellationToken ct)
    {
        var isExistAccount = false;
        if (File.Exists(CiphertextPolicy.RootFilePath)) isExistAccount = true;
        await SendAsync(new Output
        {
            IsExistAccount = isExistAccount,
            AccountName = "reformtek",
        }, cancellation: ct).ConfigureAwait(false);
    }
    public readonly struct Output
    {
        public required bool IsExistAccount { get; init; }
        public required string AccountName { get; init; }
    }
}