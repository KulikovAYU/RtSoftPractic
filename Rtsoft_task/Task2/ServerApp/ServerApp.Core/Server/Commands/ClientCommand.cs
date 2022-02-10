using System;
using Newtonsoft.Json;

namespace ServerApp.Core.Server.Commands
{
    public class ClientCommand
    {
        public ClientCommand(CommandType type)
        {
            Type = type;
        }
        
        public CommandType Type { get; set; }
        public string Name { get; set; } = "";
        public string Args { get; set; }= "";

        public Guid Guid { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
        public static ClientCommand FromJson(string str) => JsonConvert.DeserializeObject<ClientCommand>(str);
    }
}