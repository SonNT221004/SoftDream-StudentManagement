using Polly;
using StudentWeb.Resilience.ConnectionState;

namespace StudentWeb.Resilience;

public static class GrpcResilienceKeys
{
    public static readonly ResiliencePropertyKey<GrpcConnectionState> Ui =
        new("grpc-connection-ui");
}