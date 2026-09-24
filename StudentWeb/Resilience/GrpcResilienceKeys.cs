using Polly;

namespace StudentWeb.Resilience;

public static class GrpcResilienceKeys
{
    public static readonly ResiliencePropertyKey<GrpcConnectionState> Ui =
        new("grpc-connection-ui");
}