using System.Net.Sockets;

namespace LightSocket.Core;

public class TcpConnectionContext
{
    public Socket Socket { get; set; }
    public object State { get; set; }
    
    public bool IsBusy { get; set; }
    
    public int AvailableReadLength { get; set; }

    public T GetState<T>()
    {
        return (T)State;
    }
}