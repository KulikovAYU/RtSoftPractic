using System;
using ClientApp.Base;
using Newtonsoft.Json;

namespace ClientApp.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ConnectionPref : ViewModelBase
    {
        [JsonProperty]
        public int PortNumber { get; set; }

        [JsonProperty]
        public string HostNameOrAdress { get; set; }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public Guid Id { get; private set; } = Guid.NewGuid();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class MqttConnectionPref : ViewModelBase
    {
        [JsonProperty]
        public int PortNumber { get; set; }
        [JsonProperty]
        public string HostAdress { get; set; }
        [JsonProperty]
        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}
