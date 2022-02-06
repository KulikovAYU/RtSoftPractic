using System;

namespace ClientApp.Client
{
    public class ConnectionPref
    {
        public int PortNumber { get; set; }

        public string HostNameOrAdress { get; set; }

        public string UserName { get; set; }

        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}
