using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace LightSocket.Core;

public class AsyncTcpChannel
{
    private readonly TcpProtocol _initialProtocol;

    public AsyncTcpChannel(TcpProtocol initialTcpProtocol)
    {
        _initialProtocol = initialTcpProtocol;
    }
    
    public void HandleNewConnection(Socket socket, int idleTimeoutMs = -1, Func<TcpConnectionContext> onIdleTimeout = null, CancellationToken? cancellationToken = null)
    {
        TcpProtocol currentProtocol = _initialProtocol;
        socket.Blocking = false;
        socket.SendBufferSize = 10240;
        socket.ReceiveBufferSize = 10240;
        Task.Factory.StartNew(async () =>
        {
            try
            {
                var buffer = new Memory<byte>(GC.AllocateArray<byte>(10240, true));
                var lastActivityTicks = long.MinValue;
                var context = new TcpConnectionContext
                {
                    Socket = socket
                };

                currentProtocol.Init0(context);

                while (context.IsBusy
                       || idleTimeoutMs == -1
                       || socket.Available > 0
                       || (socket.Available == 0 && (DateTime.Now.Ticks - lastActivityTicks < idleTimeoutMs)))
                {
                    context.AvailableReadLength = await socket.ReceiveAsync(buffer, SocketFlags.Peek);

                    // Step 0: Peek data and inform protocol
                    currentProtocol.OnDataPeeked1(context, buffer.Slice(0, context.AvailableReadLength));

                    // Step 1: Check if available data is too much for parsing current message and get how much is enough.
                    // use-case: header is parsed and body must continue with another protocol, so reading further than necessary causes data loss.
                    // exception: excess data received can be handled by protocol itself and stored inside {TcpConnectionContext.State}.
                    var expectedLen = currentProtocol.ReceiveBytesCount2(context);

                    if (expectedLen > context.AvailableReadLength)
                    {
                        throw new IndexOutOfRangeException(
                            $"ReceiveBytesCount1() should return value lesser or equal to peeked buffer length expectedLen:{expectedLen}, readLen:{context.AvailableReadLength}");
                    }

                    var readBuffer = buffer.Slice(0, expectedLen);

                    // Step 2: Read necessary data
                    await socket.ReceiveAsync(readBuffer, SocketFlags.None);
                    await currentProtocol.OnDataReceived3(context, readBuffer).ConfigureAwait(false);

                    // Step 3: Check if protocol change is necessary 
                    TcpProtocol nextTcpProtocol = currentProtocol.ContinueWithProtocol4(context);
                    if (nextTcpProtocol != null)
                    {
                        currentProtocol = nextTcpProtocol;
                        currentProtocol.Init0(context);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
            }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness | TaskCreationOptions.RunContinuationsAsynchronously);
    }
}