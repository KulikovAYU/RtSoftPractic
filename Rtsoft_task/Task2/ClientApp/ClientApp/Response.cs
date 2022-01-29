using Newtonsoft.Json.Linq;
using System;

namespace ClientApp
{
    public class Response
    {
        public CommandType Type { get; set; }
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public bool FromJSON(string cmdJSON)
        {
            try
            {
                JObject jObject = JObject.Parse(cmdJSON);
                Type = (CommandType)int.Parse(jObject["Type"].ToString());
                StatusCode = int.Parse(jObject["StatusCode"].ToString());
                Body = jObject["Body"].ToString();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"Type = {Type}; StatusCode = {StatusCode}; Body = {Body}";
        }
    }
}
