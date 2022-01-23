using System.Net;

namespace ServerApp.Core
{
    public class SocketPrefs
    {
        public class Builder
        {
            public Builder SetHostNameOrAdress(string hostNameOrAdress)
            {
                activePref.IpHostInfo = Dns.GetHostEntry(hostNameOrAdress);
                return this;
            }

            public Builder SetPortNum(int portNumber)
            {
                activePref.PortNumber = portNumber;
                return this;
            }

            public Builder SetIp(IPAddress ipAddress)
            {
                activePref.IpAddress = ipAddress;
                return this;
            }

            public Builder SetMaxConnections(int maxConn)
            {
                activePref.MaxConnections = maxConn;
                return this;
            }

            public SocketPrefs Build() => activePref;

            private SocketPrefs activePref = new SocketPrefs();
        }

        public static Builder NewBuilder() => new Builder();

        private SocketPrefs() { }

        public IPHostEntry IpHostInfo { get; private set; }

        public int PortNumber { get; private set; }

        public IPAddress IpAddress { get; private set; }

        public int MaxConnections { get; private set; } = 0;


        public override string ToString()
        {
            return $"{IpHostInfo.HostName}, ip: {IpAddress}, port: {PortNumber}, max conn: {MaxConnections}";
        }

    }
}