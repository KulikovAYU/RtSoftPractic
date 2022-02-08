using System.Net;
using SysMonitor.Interfaces;

namespace SysMonitor
{
    class SocketPrefs : ISocketPrefs
    {
        public class Builder
        {
            public Builder SetPortNum(int portNumber)
            {
                activePref.PortNumber = portNumber;
                return this;
            }

            public Builder SetIp(string ipAddress)
            {
                activePref.IpAddress = System.Net.IPAddress.Parse(ipAddress);
                return this;
            }

            public Builder SetMaxConnections(int maxConn)
            {
                activePref.MaxConnections = maxConn;
                return this;
            }

            public Builder UserName(string userName)
            {
                activePref.UserName = userName;
                return this;
            }

            public Builder Password(string userPwd)
            {
                activePref.UserPassword = userPwd;
                return this;
            }

            public SocketPrefs Build() => activePref;

            private SocketPrefs activePref = new SocketPrefs();
        }

        public static Builder NewBuilder() => new Builder();

        private SocketPrefs() { }

        public int PortNumber { get; private set; }

        public IPAddress IpAddress { get; private set; }

        public int MaxConnections { get; private set; } = 0;

        public string UserName { get; private set; }

        public string UserPassword { get; private set; }

        public override string ToString()
        {
            return $"ip: {IpAddress}, port: {PortNumber}, max conn: {MaxConnections}";
        }
    }
}