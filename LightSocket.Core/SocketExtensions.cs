using System.Net.Sockets;

namespace LightSocket.Core;

public static class SocketExtensions
{
    public static ValueTask<int> WriteAllAsync(this Socket socket, byte[] bytes)
    {
        return socket.WriteAllAsync(bytes.AsMemory());
    }

    public static async ValueTask<int> WriteAllAsync(this Socket socket, ReadOnlyMemory<byte> bytes)
    {
        int dataLength = bytes.Length;
        int writeLen = 0;
        while (writeLen < dataLength)
        {
            writeLen += await socket.SendAsync(bytes, SocketFlags.None).ConfigureAwait(false);
            if (writeLen < bytes.Length)
            {
                bytes = bytes.Slice(writeLen, bytes.Length - writeLen);
            }
        }
        return writeLen;
    }

    public static ValueTask<int> WriteAllAsync(this Socket socket, ArraySegment<byte> bytes)
    {
        return socket.WriteAllAsync(bytes.AsMemory());
    }
}