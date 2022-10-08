namespace TCPFileServer.DataAccess;

public sealed class BorrowableFileStream : Stream
{
    private DataHandler _dataHandler;
    private FileStream _stream;

    public BorrowableFileStream(FileStream stream, DataHandler dataHandler)
    {
        _stream = stream;
        _dataHandler = dataHandler;
    }


    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        return _stream.Read(buffer);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException("You're trying to write to a read-only stream...");
        _stream.Write(buffer, offset, count);
    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;
    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    protected override void Dispose(bool dispose)
    {
        _stream.Close();
        _dataHandler.Release();
    }

    public override void Flush()
    {
        _stream.Flush();
    }
}