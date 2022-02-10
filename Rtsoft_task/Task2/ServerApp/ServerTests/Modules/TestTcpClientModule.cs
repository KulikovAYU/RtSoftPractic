using System.Net.Sockets;
using Autofac;
using ServerApp.Core;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server;

namespace ServerTests.Modules;

class TestTcpClientModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TcpClient>();
        builder.Register(pr => SocketPrefs.NewBuilder()
            .SetIp("127.0.0.1")
            .SetMaxConnections(10)
            .SetPortNum(11000)
            .Build())
            .As<ISocketPrefs>();
    }
}