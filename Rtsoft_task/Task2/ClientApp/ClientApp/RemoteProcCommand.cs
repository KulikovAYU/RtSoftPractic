using Newtonsoft.Json;

namespace ClientApp
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    /// <summary>
    /// Command which represents remote process
    /// </summary>
    public class RemoteProcCommand
    {
        public RemoteProcCommand(CommandType type)
        {
            Type = type;
        }
        
        public CommandType Type { get; set; }
        public string Name { get; set; }
        public string Args { get; set; }

        public string ToJSON() => JsonConvert.SerializeObject(this);
    }
}
