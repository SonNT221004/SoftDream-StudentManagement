using Polly.Registry;
using StudentWeb.Resilience;
using StudentWeb.Resilience.ConnectionState;
using StudentWeb.Resilience.MyInterceptor;

public sealed class ClassGrpcRetryInterceptor : GrpcRetryInterceptor
{
    public ClassGrpcRetryInterceptor(
        ResiliencePipelineProvider<string> pipelineProvider,
        ClassGrpcConnectionState connection)
        : base(
            pipelineProvider,
            connection,
            GrpcPipelineNames.Class)
    {
    }
}