using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ABOBAEngine.Utils;

public sealed class LineByLineReader : IDisposable
{
    public bool IsEmpty { get; private set; }
    private int _position;
    private TextBuffer _inactiveBuffer => _activeBuffer.Equals(_buffers[0]) ? _buffers[1] : _buffers[0];
    private TextBuffer _activeBuffer;
    
    private readonly TextBuffer[] _buffers;
    private readonly int _bufferLength;
    private readonly FileStream _fileStream;
    private readonly byte[] _byteBuffer;

    private Func<ReadOnlyMemory<char>> LineReadDelegate;
    
    public LineByLineReader(FileStream fileStream, int bufferLength = 500000)
    {
        _fileStream = fileStream;
        _bufferLength = bufferLength;
        _byteBuffer = new byte[bufferLength];
        
        _buffers = new[] { new TextBuffer(bufferLength), new TextBuffer(bufferLength) };
        _activeBuffer = _buffers[0];
        InitializeBuffers();
    }

    public ReadOnlyMemory<char> ReadLine()
    {
        ReadOnlyMemory<char> result = LineReadDelegate.Invoke();
        _position += result.Length+1;
        if (_position >= _bufferLength - 1) SwapBuffers();
        return result;
    }

    private ReadOnlyMemory<char> GetNextLine()
    {
        ReadOnlySpan<char> span = _activeBuffer.ReadOnlyMemory.Span;
        int iteratorPosition = _position;
        while (span[iteratorPosition] != '\n')
        {
            if (iteratorPosition + 1 >= _bufferLength) return GetNextLineOnVerge();
            iteratorPosition++;
        }
        var result = _activeBuffer.ReadOnlyMemory.Slice(_position, iteratorPosition - _position);
        return result;
    }
    

    private ReadOnlyMemory<char> GetNextLineWithChecks()
    {
        ReadOnlySpan<char> span = _activeBuffer.ReadOnlyMemory.Span;
        int iteratorPosition = _position;
        while (span[iteratorPosition] != '\n')
        {
            if (iteratorPosition >= _activeBuffer.Length - 1)
            {
                IsEmpty = true;
                break;
            }
            if (iteratorPosition + 1 >= _bufferLength) return GetNextLineOnVerge();
            iteratorPosition++;
        }
        var result = _activeBuffer.ReadOnlyMemory.Slice(_position, iteratorPosition - _position);

        return result;
    }

    private ReadOnlyMemory<char> GetNextLineOnVerge()
    {
        ReadOnlySpan<char> span = _activeBuffer.ReadOnlyMemory.Span;
        List<char> result = new List<char>();
        int startPosition = _position;
        int iteratorPosition;

        for (iteratorPosition = startPosition; iteratorPosition < _bufferLength; iteratorPosition++)
        {
            if (span[iteratorPosition] == '\n') return new ReadOnlyMemory<char>(result.ToArray());
            result.Add(span[iteratorPosition]);
        }
        SwapBuffers();
        ReadOnlyMemory<char> afterTear = LineReadDelegate.Invoke();
        result.AddRange(afterTear.ToArray());

        return new ReadOnlyMemory<char>(result.ToArray());
    }

    private void SwapBuffers()
    {
        ReadToBuffer(_inactiveBuffer);
        LineReadDelegate = _inactiveBuffer.IsFull ? GetNextLine : GetNextLineWithChecks;
        
        _activeBuffer = _activeBuffer.Equals(_buffers[0]) ? _buffers[1] : _buffers[0];
        _position = 0;
    }
    
    private void InitializeBuffers()
    {
        ReadToBuffer(_buffers[0]);
        LineReadDelegate = _buffers[0].IsFull ? GetNextLine : GetNextLineWithChecks;
    }
    
    private void ReadToBuffer(TextBuffer buffer)
    {
        Span<byte> byteBuffer = new Span<byte>(_byteBuffer);
        int readLength = _fileStream.Read(byteBuffer);
        _fileStream.Flush();
        buffer.Length = readLength;
        Encoding.UTF8.GetChars(byteBuffer, buffer.Memory.Span);
    }
    
    public void Dispose()
    {
        _fileStream.Dispose();
    }

    private sealed class TextBuffer
    {
        public ReadOnlyMemory<char> ReadOnlyMemory => Memory;
        public bool IsFull => Length == _capacity;
        public int Length;
        public readonly Memory<char> Memory;
        
        private readonly int _capacity;

        public TextBuffer(int bufferLength)
        {
            _capacity = bufferLength;
            Memory = new Memory<char>(new char[bufferLength]);
        }
    }
}
