using System;
using Newtonsoft.Json.Linq;

namespace ClientApp.Client
{
    public class Response
    {
        private Guid _guid = Guid.Empty;
        public CommandType Type { get; private set; } = CommandType.eUndef;
        public int StatusCode { get; private set; }
        public string Body { get; private set; }

        public Guid Guid
        {
            get => _guid;
            private set => _guid = value;
        }

        public bool FromJSON(string cmdJSON)
        {
            try
            {
                JObject jObject = JObject.Parse(cmdJSON);
                Type = (CommandType)int.Parse(jObject["Type"].ToString());
                StatusCode = int.Parse(jObject["StatusCode"].ToString());
                Body = jObject["Body"].ToString();
                Guid.TryParse(jObject["Guid"]?.ToString(), out _guid);

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