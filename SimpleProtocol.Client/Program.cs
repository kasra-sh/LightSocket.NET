// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;

Console.WriteLine("Hello, World!");

var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
socket.Connect(new IPAddress(new byte[] {127, 0, 0, 1}), 5550);
var i = 0;
Int32 size = 60000;
var bytes = new byte[size];
Random.Shared.NextBytes(bytes);
var data = new Memory<byte>(bytes);
var rcvBytes = new byte[60000];
var rcvdata = new Memory<byte>(new byte[60000]);
socket.Blocking = true;

// var sizeBytes = new ArraySegment<byte>(BitConverter.GetBytes(size));
Task.Run(async () =>
{
    try
    {
        while (true)
        {
            // await socket.SendAsync(sizeBytes, SocketFlags.None);
            var sends = await socket.SendAsync(data, SocketFlags.None);
            // Console.WriteLine("Sent {0} bytes,{1}", sends, i++);
            // if (socket.Available>0)
            await socket.ReceiveAsync(rcvBytes, SocketFlags.None);
            // await Task.Delay(1000);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
   
})
    .GetAwaiter().GetResult();
