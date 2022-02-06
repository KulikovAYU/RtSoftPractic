using Newtonsoft.Json;
using ServerApp.Core.TcpServer.Commands;
using System;

namespace ServerApp.Core.TcpServer
{
    public class Response
    {
        public Response(CommandType type, int statusCode, string body = "")
        {
            Type = type;
            StatusCode = statusCode;
            Body = body;
        }

        public CommandType Type { get; private set; }
        public int StatusCode { get; private set; }
        public string Body { get; private set; }
     

        public virtual  string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class CommandResponse : Response 
    {
        public CommandResponse(Guid guid ,CommandType type, int statusCode, string body = "") : base(type, statusCode, body)
        {
            Guid = guid;
        }

        public Guid Guid { get; }
        

    }
}
