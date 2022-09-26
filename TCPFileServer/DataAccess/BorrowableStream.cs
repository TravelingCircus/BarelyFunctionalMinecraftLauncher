namespace HTTPFileServer.DataAccess;

public sealed class BorrowableReadonlyStream : FileStream
{
    private TaskCompletionSource _releaseSource;
    
    public BorrowableReadonlyStream(string path, TaskCompletionSource releaseSource) : base(path, FileMode.Open)
    {
        _releaseSource = releaseSource;
    }

    protected override void Dispose(bool dispose)
    {
        _releaseSource.SetResult();
        base.Dispose(dispose);
    }
}