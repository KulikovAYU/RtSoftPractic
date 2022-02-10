using System.Net.Sockets;
using ServerApp.Core.Interfaces;

namespace ServerApp.Core.Server
{
    class ConnectedClient : IConnectedClient
    {
        public TcpClient ClientData { get;private set; }

        public string Name { get; set; }

        public ConnectedClient(TcpClient newClient)
        {
            ClientData = newClient;
        }

        public override string ToString() => $"{Name}; Data: {ClientData.Client.RemoteEndPoint}";
    }
}
