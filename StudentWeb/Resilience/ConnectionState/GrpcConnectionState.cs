namespace StudentWeb.Resilience.ConnectionState;

public class GrpcConnectionState
{
    public bool IsRetrying { get; private set; }
    public bool IsUnavailable { get; private set; }
    public int Attempt { get; private set; }
    public int MaxAttempts { get; private set; }
    public TimeSpan RetryDelay { get; private set; }
    public string? Detail { get; private set; }

    public event Action? Changed;

    public void BeginCall()
    {
        IsRetrying = false;
        IsUnavailable = false;
        Detail = null;
        Notify();
    }

    public void ReportRetry(int attempt, int maxAttempts, TimeSpan delay, string? detail)
    {
        IsRetrying = true;
        IsUnavailable = false;
        Attempt = attempt;
        MaxAttempts = maxAttempts;
        RetryDelay = delay;
        Detail = detail;
        Notify();
    }

    public void ReportSuccess()
    {
        IsRetrying = false;
        IsUnavailable = false;
        Detail = null;
        Notify();
    }

    public void ReportFailed(string message)
    {
        IsRetrying = false;
        IsUnavailable = true;
        Detail = message;
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}