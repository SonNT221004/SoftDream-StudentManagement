using Grpc.Core;
using Grpc.Core.Interceptors;
using Polly;
using Polly.Registry;

namespace StudentWeb.Resilience;

public sealed class GrpcRetryInterceptor : Interceptor
{
    public const string PipelineName = "grpc-rpc-retry";

    private readonly ResiliencePipeline _pipeline;
    private readonly GrpcConnectionState _ui;

    public GrpcRetryInterceptor(
        ResiliencePipelineProvider<string> pipelineProvider,
        GrpcConnectionState ui)
    {
        _pipeline = pipelineProvider.GetPipeline(PipelineName);
        _ui = ui;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var responseTask = ExecuteWithRetryAsync(request, context, continuation);
        return new AsyncUnaryCall<TResponse>(
            responseTask,
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, string.Empty),
            () => new Metadata(),
            () => { });
    }

    private async Task<TResponse> ExecuteWithRetryAsync<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        where TRequest : class
        where TResponse : class
    {
        var resilienceContext = ResilienceContextPool.Shared.Get(context.Options.CancellationToken);
        resilienceContext.Properties.Set(GrpcResilienceKeys.Ui, _ui);
        _ui.BeginCall();

        try
        {
            var result = await _pipeline.ExecuteAsync(
                async (ctx, ct) =>
                {
                    var options = context.Options.WithCancellationToken(ct);
                    var retryContext = new ClientInterceptorContext<TRequest, TResponse>(
                        context.Method,
                        context.Host,
                        options);
                    var call = continuation(request, retryContext);
                    return await call.ResponseAsync.ConfigureAwait(false);
                },
                resilienceContext).ConfigureAwait(false);

            _ui.ReportSuccess();
            return result;
        }
        catch (Exception ex)
        {
            _ui.ReportFailed(ex.Message);
            throw;
        }
        finally
        {
            ResilienceContextPool.Shared.Return(resilienceContext);
        }
    }
}