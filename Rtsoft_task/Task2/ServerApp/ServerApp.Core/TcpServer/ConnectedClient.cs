using System.Net.Sockets;

namespace ServerApp.Core.TcpServer
{
    class ConnectedClient
    {
        public TcpClient ClientData { get;private set; }

        public string Name { get; set; }

        public ConnectedClient(TcpClient newClient)
        {
            ClientData = newClient;
        }

        public override string ToString()
        {
            return $"{Name}; Data: {ClientData.Client.RemoteEndPoint}";
        }
    }
}
