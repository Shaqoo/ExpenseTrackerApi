namespace ExpenseTracker.Extensions;

public class ApplicationStartupState
{
    private readonly TaskCompletionSource _applicationStarted = new();

    public Task ApplicationStarted => _applicationStarted.Task;

    public void SetApplicationStarted() => _applicationStarted.TrySetResult();
}