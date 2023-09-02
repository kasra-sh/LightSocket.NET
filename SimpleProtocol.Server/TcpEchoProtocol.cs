using LightSocket.Core;

namespace LightSocket.Server;

public struct EchoProtocolState
{
    public int Length = 0;
    public int TotalTraffic = 0;

    public EchoProtocolState()
    {
    }
}

public class TcpEchoProtocol: TcpProtocol
{
    private static long lastConsoleWrite = DateTime.Now.Ticks;
    private static long total = 0;
    protected override async ValueTask SendProtocolMessageAsync(TcpConnectionContext ctx, byte[] message)
    {
        await ctx.Socket.WriteAllAsync(message).ConfigureAwait(false);
    }

    public override void Init0(TcpConnectionContext ctx)
    {
        ctx.State = new EchoProtocolState();
    }

    public override void OnDataPeeked1(TcpConnectionContext ctx, ReadOnlyMemory<byte> input)
    {
        var state = ctx.GetState<EchoProtocolState>();
        state.Length = input.Length;
        total += state.Length;
        if (DateTime.Now.Ticks - lastConsoleWrite > 1000)
        {
            lastConsoleWrite = DateTime.Now.Ticks;
            Console.WriteLine("Total traffic: "+total/(1024*1024));
        }
    }

    public override int ReceiveBytesCount2(TcpConnectionContext ctx)
    {
        return ctx.GetState<EchoProtocolState>().Length;
    }

    public override async ValueTask OnDataReceived3(TcpConnectionContext ctx, ReadOnlyMemory<byte> input)
    {
        await SendAsync(ctx, input.ToArray()).ConfigureAwait(false);
    }
    
    public override TcpProtocol ContinueWithProtocol4(TcpConnectionContext ctx)
    {
        return null;
    }
}