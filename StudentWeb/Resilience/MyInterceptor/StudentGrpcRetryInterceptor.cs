using Polly.Registry;
using StudentWeb.Resilience.ConnectionState;

namespace StudentWeb.Resilience.MyInterceptor;

public sealed class StudentGrpcRetryInterceptor : GrpcRetryInterceptor
{
    public StudentGrpcRetryInterceptor(
        ResiliencePipelineProvider<string> pipelineProvider,
        StudentGrpcConnectionState connection)
        : base(
            pipelineProvider,
            connection,
            GrpcPipelineNames.Student)
    {
    }
}