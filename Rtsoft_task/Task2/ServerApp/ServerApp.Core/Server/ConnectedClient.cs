using System.Net.Sockets;

namespace ServerApp.Core.Server
{
    class ConnectedClient
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
