namespace LightSocket.Core;

public abstract class TcpProtocol
{
    public Action<TcpProtocol, object> OnMessageReceivedCallback { get; set; }

    protected abstract ValueTask SendProtocolMessageAsync(TcpConnectionContext ctx, byte[] message);
    
    public async ValueTask SendAsync(TcpConnectionContext ctx, byte[] message)
    {
        try
        {
            ctx.IsBusy = true;
            await SendProtocolMessageAsync(ctx, message).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            ctx.IsBusy = false;
        }
    }

    public abstract void Init0(TcpConnectionContext ctx);
    public abstract void OnDataPeeked1(TcpConnectionContext ctx, ReadOnlyMemory<byte> input);
    public abstract int ReceiveBytesCount2(TcpConnectionContext ctx);
    public abstract ValueTask OnDataReceived3(TcpConnectionContext ctx, ReadOnlyMemory<byte> input);
    /// <summary>
    /// If needs to change protocol from this point, return new protocol instance, otherwise return null
    /// </summary>
    /// <returns>Next TcpProtocol or null</returns>
    public abstract TcpProtocol ContinueWithProtocol4(TcpConnectionContext ctx);
}