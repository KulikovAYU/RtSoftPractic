using System.Net;

namespace ServerApp.Core.Interfaces
{
    public interface ISocketPrefs
    {
        int PortNumber { get; }
        IPAddress IpAddress { get; }
        int MaxConnections { get; }
        string UserName { get; }
        string UserPassword { get; }
    }
}