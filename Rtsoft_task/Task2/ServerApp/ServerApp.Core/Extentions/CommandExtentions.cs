using Newtonsoft.Json;
using ServerApp.Core.Server;

namespace ServerApp.Core.Extentions
{
    public static class CommandExtentions
    {
        public static string ToJson(this Response response) => JsonConvert.SerializeObject(response);
        
        public static string ToJson(this CommandResponse response) => JsonConvert.SerializeObject(response);
        
        public static Response FromJson(this Response response, string jResp) => JsonConvert.DeserializeObject<Response>(jResp);
        public static CommandResponse FromJson(this CommandResponse response, string jResp) => JsonConvert.DeserializeObject<CommandResponse>(jResp);
       
    }
}