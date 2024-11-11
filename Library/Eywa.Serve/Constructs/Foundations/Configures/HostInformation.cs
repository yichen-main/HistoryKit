namespace Eywa.Serve.Constructs.Foundations.Configures;
public sealed class HostInformation
{
    public required TextHTTP HTTP { get; init; }
    public required TextHTTPS HTTPS { get; init; }
    public required TextSMTP SMTP { get; init; }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct TextHTTP
    {
        public required int Port { get; init; }
        public required TextPath Path { get; init; }
        public readonly struct TextPath
        {
            public required string SOAP { get; init; }
            public required string GraphQL { get; init; }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct TextHTTPS
    {
        public required int Port { get; init; }
        public required bool Enabled { get; init; }
        public required TextCertificate Certificate { get; init; }
        public readonly struct TextCertificate
        {
            public required string Location { get; init; }
            public required string Password { get; init; }
        }
    }
    public readonly struct TextSMTP
    {
        public required int Port { get; init; }
        public required int PoolSize { get; init; }
        public required string Host { get; init; }
        public required string Sender { get; init; }
        public required string Password { get; init; }
        public required string LoginPageUrl { get; init; }
    }
}