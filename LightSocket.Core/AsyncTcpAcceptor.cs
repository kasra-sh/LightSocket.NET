using System.Net;
using System.Net.Sockets;

namespace LightSocket.Core;

public class AsyncSocketAcceptor: IDisposable
{
    private int _port = 0;
    private Socket _serverSocket;
    private bool _stopped = false;

    public AsyncSocketAcceptor()
    {
    }

    public async Task StartAsync(int port, Action<Socket> onSocketAccepted, CancellationToken? cancellationToken = null)
    {
        if (_port != 0)
        {
            throw new InvalidOperationException($"SocketListener is already running on port {_port}, create a new instance or call Stop() before starting again.");
        }

        _port = port;

        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);
        _serverSocket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Blocking = false;
        _serverSocket.Bind(localEndPoint);
        _serverSocket.Listen();
        
        try
        {
            while (!_stopped && !(cancellationToken?.IsCancellationRequested ?? false))
            {
                Socket socket;
                if (cancellationToken != null)
                {
                    socket = await _serverSocket.AcceptAsync();
                }
                else
                {
                    socket = await _serverSocket.AcceptAsync(cancellationToken.GetValueOrDefault());
                }
                try
                {
                    onSocketAccepted.Invoke(socket);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        TryCloseSocket();
    }

    public void Stop()
    {
        _port = 0;
        _stopped = true;
        TryCloseSocket();
    }
    
    private void TryCloseSocket()
    {
        try
        {
            _serverSocket.Close(1);
        }
        catch
        {
            // ignored
        }
    }

    public void Dispose()
    {
        TryCloseSocket();
        _serverSocket?.Dispose();
    }
}
