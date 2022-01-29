using Newtonsoft.Json;
using ServerApp.Core.Commands;

namespace ServerApp.Core
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

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
