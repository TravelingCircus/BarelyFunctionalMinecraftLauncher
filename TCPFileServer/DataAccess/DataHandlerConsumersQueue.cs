using System.Collections.Concurrent;

namespace TCPFileServer.DataAccess;

public class DataHandlerConsumersQueue<T> where T : DataHandler
{
    private TaskCompletionSource _releaseSource;
    private readonly T _handler;
    private readonly ConcurrentQueue<TaskCompletionSource<T>> _queue;

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
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_queue.TryDequeue(out TaskCompletionSource<T> queuedConsumer)) continue;

            await BorrowHandler(queuedConsumer).ConfigureAwait(false);
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