using Newtonsoft.Json;
using ServerApp.Core.TcpServer.Commands;

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

        public CommandType Type { get; set; }
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
