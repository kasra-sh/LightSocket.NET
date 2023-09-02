// See https://aka.ms/new-console-template for more information

using LightSocket.Core;
using LightSocket.Server;

var asyncTcpChannel = new AsyncTcpChannel(new TcpEchoProtocol());

await new AsyncSocketAcceptor().StartAsync(5550, socket =>
{
    asyncTcpChannel.HandleNewConnection(socket, 120*1000);
});
