using System;
using Newtonsoft.Json;
using ServerApp.Core.Server.Commands;

namespace ServerApp.Core.Server
{
    public class Response
    {
        public CommandType Type { get; set; }
        public int StatusCode { get; set; }
        public string Body { get; set; }
        
        public Response(CommandType type, int statusCode, string body = "")
        {
            Type = type;
            StatusCode = statusCode;
            Body = body;
        }

        // public virtual string ToJson() => JsonConvert.SerializeObject(this);
     
    }

    public class CommandResponse : Response 
    {
        public CommandResponse(Guid guid ,CommandType type, int statusCode, string body = "") : base(type, statusCode, body)
        {
            Guid = guid;
        }

        public Guid Guid { get; }
        

        // public override string ToJson() => JsonConvert.SerializeObject(this);
      
    }
}
