using System.Collections.Concurrent;

namespace HTTPFileServer.DataAccess;

public class DataHandlerConsumersQueue<T> where T : DataHandler
{
    private T _handler;
    private ConcurrentQueue<TaskCompletionSource<T>> _queue;

    public DataHandlerConsumersQueue(T handler)
    {
        _queue = new ConcurrentQueue<TaskCompletionSource<T>>();
        _handler = handler;
    }

    public Task<T> GetDataHandler()
    {
        TaskCompletionSource<T> consumer = new TaskCompletionSource<T>();
        _queue.Enqueue(consumer);
        return consumer.Task;
    }

    public async Task RunBlocking(CancellationToken cancellationToken)
    {
        TaskCompletionSource<T> consumer = null;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (consumer is null && !_queue.TryDequeue(out consumer)) continue;

            TaskCompletionSource releaseSource = new TaskCompletionSource();
            _handler.Borrow(releaseSource);
            consumer.SetResult(_handler);
            await releaseSource.Task;
        }
    }
}