using Polly.Registry;
using StudentWeb.Resilience.ConnectionState;

namespace StudentWeb.Resilience.MyInterceptor;

public sealed class AnalyticsGrpcRetryInterceptor : GrpcRetryInterceptor
{
    public AnalyticsGrpcRetryInterceptor(
        ResiliencePipelineProvider<string> pipelineProvider,
        AnalyticsGrpcConnectionState connection)
        : base(
            pipelineProvider,
            connection,
            GrpcPipelineNames.Analytics)
    {
    }
}