namespace LightSocket.Core;

public class MockHttpDownloadProtocol: TcpProtocol
{
    private readonly int _size;
    public MockHttpDownloadProtocol(int size)
    {
        _size = size;
    }
    protected override ValueTask SendProtocolMessageAsync(TcpConnectionContext ctx, byte[] message)
    {
        throw new NotImplementedException();
    }

    public override void Init0(TcpConnectionContext ctx)
    {
        ctx.State = 0;
    }

    public override void OnDataPeeked1(TcpConnectionContext ctx, ReadOnlyMemory<byte> input)
    {
    }

    public override int ReceiveBytesCount2(TcpConnectionContext ctx)
    {
        return ctx.Socket.Available;
    }

    public override ValueTask OnDataReceived3(TcpConnectionContext ctx, ReadOnlyMemory<byte> input)
    {
        throw new NotImplementedException();
    }

    public override TcpProtocol ContinueWithProtocol4(TcpConnectionContext ctx)
    {
        throw new NotImplementedException();
    }
}