using Grpc.Core;
using Grpc.Net.ClientFactory;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using StudentWeb.Resilience.ConnectionState;
using StudentWeb.Resilience.MyInterceptor;

namespace StudentWeb.Resilience;

public static class GrpcClientRetryExtensions
{
    public static IServiceCollection AddGrpcRpcRetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var maxRetryAttempts = configuration.GetValue("Grpc:Retry:MaxRetryAttempts", 3);
        var delayMilliseconds = configuration.GetValue("Grpc:Retry:DelayMilliseconds", 200);

        services.AddScoped<StudentGrpcConnectionState>();
        services.AddScoped<ClassGrpcConnectionState>();
        services.AddScoped<AnalyticsGrpcConnectionState>();

        services.AddScoped<StudentGrpcRetryInterceptor>();
        services.AddScoped<ClassGrpcRetryInterceptor>();
        services.AddScoped<AnalyticsGrpcRetryInterceptor>();

        services.AddResiliencePipeline(
    GrpcPipelineNames.Student,
    (pipeline,_) => ConfigureGrpcPipeline(pipeline, maxRetryAttempts, delayMilliseconds));

        services.AddResiliencePipeline(
            GrpcPipelineNames.Class,
            (pipeline, _) => ConfigureGrpcPipeline(pipeline, maxRetryAttempts, delayMilliseconds));

        services.AddResiliencePipeline(
            GrpcPipelineNames.Analytics,
            (pipeline, _) => ConfigureGrpcPipeline(pipeline, maxRetryAttempts, delayMilliseconds));


        return services;
    }
    public static IHttpClientBuilder AddStudentGrpcRetry(
       this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddInterceptor<StudentGrpcRetryInterceptor>(
            InterceptorScope.Client);
    }

    public static IHttpClientBuilder AddClassGrpcRetry(
        this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddInterceptor<ClassGrpcRetryInterceptor>(
            InterceptorScope.Client);
    }

    public static IHttpClientBuilder AddAnalyticsGrpcRetry(
        this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddInterceptor<AnalyticsGrpcRetryInterceptor>(
            InterceptorScope.Client);
    }

    private static bool IsTransient(StatusCode statusCode) =>
        statusCode is StatusCode.Unavailable
            or StatusCode.DeadlineExceeded
            or StatusCode.ResourceExhausted
            or StatusCode.Aborted
            or StatusCode.Internal;
    private static void ConfigureGrpcPipeline(
    ResiliencePipelineBuilder pipeline,
    int maxRetryAttempts,
    int delayMilliseconds)
    {
        pipeline
     .AddRetry(new RetryStrategyOptions
     {
         MaxRetryAttempts = maxRetryAttempts,
         Delay = TimeSpan.FromMilliseconds(delayMilliseconds),
         BackoffType = DelayBackoffType.Exponential,
         UseJitter = true,
         ShouldHandle = new PredicateBuilder()
             .Handle<RpcException>(ex => IsTransient(ex.StatusCode)),
         OnRetry = args =>
         {
             Console.WriteLine(
                 $"Retry {args.AttemptNumber + 1}/{maxRetryAttempts}");

             return default;
         }
     })
     .AddCircuitBreaker(new CircuitBreakerStrategyOptions
     {
         FailureRatio = 1.0,
         SamplingDuration = TimeSpan.FromSeconds(30),
         MinimumThroughput = 2,
         BreakDuration = TimeSpan.FromSeconds(20),
         ShouldHandle = new PredicateBuilder()
             .Handle<RpcException>(ex => IsTransient(ex.StatusCode))
     });
    }
}