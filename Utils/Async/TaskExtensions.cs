namespace Utils.Async;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        FireAndForget(task, Console.WriteLine);
    }
        
    public static void FireAndForget(this Task task, Action<Exception> errorHandler)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted && errorHandler is not null) errorHandler(t.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}