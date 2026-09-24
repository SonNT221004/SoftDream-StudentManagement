using Grpc.Core;
using Grpc.Net.ClientFactory;
using Polly;
using Polly.Retry;

namespace StudentWeb.Resilience;

public static class GrpcClientRetryExtensions
{
    public static IServiceCollection AddGrpcRpcRetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var maxRetryAttempts = configuration.GetValue("Grpc:Retry:MaxRetryAttempts", 3);
        var delayMilliseconds = configuration.GetValue("Grpc:Retry:DelayMilliseconds", 200);

        services.AddScoped<GrpcConnectionState>();
        services.AddScoped<GrpcRetryInterceptor>();

        services.AddResiliencePipeline(GrpcRetryInterceptor.PipelineName, (pipeline, context) =>
        {
            pipeline.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = maxRetryAttempts,
                Delay = TimeSpan.FromMilliseconds(delayMilliseconds),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder()
                    .Handle<RpcException>(ex => IsTransient(ex.StatusCode)),
                OnRetry = args =>
                {
                    var status = (args.Outcome.Exception as RpcException)?.StatusCode;
                    if (args.Context.Properties.TryGetValue(GrpcResilienceKeys.Ui, out var ui))
                    {
                        ui.ReportRetry(
                            args.AttemptNumber + 1,
                            maxRetryAttempts,
                            args.RetryDelay,
                            $"Status={status}");
                    }

                    Console.WriteLine(
                        $"gRPC transient retry {args.AttemptNumber + 1}/{maxRetryAttempts} in {args.RetryDelay}. Status={status}");
                    return default;
                }
            });
        });

        return services;
    }

    public static IHttpClientBuilder AddGrpcTransientRetry(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddInterceptor<GrpcRetryInterceptor>(InterceptorScope.Client);
    }

    private static bool IsTransient(StatusCode statusCode) =>
        statusCode is StatusCode.Unavailable
            or StatusCode.DeadlineExceeded
            or StatusCode.ResourceExhausted
            or StatusCode.Aborted
            or StatusCode.Internal;
}