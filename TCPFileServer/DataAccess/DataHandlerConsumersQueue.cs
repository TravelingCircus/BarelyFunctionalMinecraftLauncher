using System.Collections.Concurrent;

namespace HTTPFileServer.DataAccess;

public class DataHandlerConsumersQueue<T> where T : DataHandler
{
    private T _handler;
    private ConcurrentQueue<TaskCompletionSource<T>> _queue;
    private TaskCompletionSource _releaseSource;

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
        TaskCompletionSource<T> consumer;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_queue.TryDequeue(out TaskCompletionSource<T> queuedConsumer)) continue;
            consumer = queuedConsumer;
            
            await BorrowHandler(consumer).ConfigureAwait(false);
        }
    }

    private Task BorrowHandler(TaskCompletionSource<T> consumer)
    {
        _releaseSource = new TaskCompletionSource();
        _handler.Borrow(_releaseSource);
            
        consumer.SetResult(_handler);
        return _releaseSource.Task;
    }
}