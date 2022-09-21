﻿using System.Net;
using System.Net.Sockets;

namespace HTTPFileServer;

public class Server
{
    public bool IsRunning { get; private set; }
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TcpListener _tcpListener;
    
    public Server()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _tcpListener = new TcpListener(IPAddress.Any, 69);
    }

    public void Start()
    {
        _tcpListener.Start();
        CancellationToken cancellationToken = _cancellationTokenSource.Token;
        IsRunning = true;
        Console.WriteLine($"STARTED thread_{Thread.CurrentThread.ManagedThreadId}");
        Task.Run(() =>
        {
            AcceptClientsBlocking(cancellationToken);
        }, cancellationToken);
    }

    private void AcceptClientsBlocking(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"LISTENING thread_{Thread.CurrentThread.ManagedThreadId}");
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            if(tcpClient.Connected)Console.WriteLine($"RECEIVED CONNECTION thread_{Thread.CurrentThread.ManagedThreadId}");
            Task.Run(() =>
            {
                HandleClient(tcpClient);
            }, cancellationToken);
        }
        Console.WriteLine($"STOPPED LISTENING thread_{Thread.CurrentThread.ManagedThreadId}");
    }
    
    private void HandleClient(TcpClient client)
    {
        NetworkStream networkStream = client.GetStream();
        Console.WriteLine($"HANDLING CONNECTION thread_{Thread.CurrentThread.ManagedThreadId}");
    }

    public void Terminate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _tcpListener.Stop();
        IsRunning = false;
        Console.WriteLine($"TERMINATED thread_{Thread.CurrentThread.ManagedThreadId}");
    }
}