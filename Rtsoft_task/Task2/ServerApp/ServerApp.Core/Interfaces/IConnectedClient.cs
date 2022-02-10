using System.Net.Sockets;

namespace ServerApp.Core.Interfaces
{
    public interface IConnectedClient
    {
        TcpClient ClientData { get; }
        string Name { get; set; }
    }
}