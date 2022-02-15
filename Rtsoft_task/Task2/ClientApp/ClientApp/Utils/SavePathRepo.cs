using System.IO;
using ClientApp.Client;

namespace ClientApp.Utils
{
    public static class SavePathRepo
    {
        public static string GetJsonPath(MqttConnectionPref pref)=> $@"{Directory.GetCurrentDirectory()}\{nameof(MqttConnectionPref)}.json";
        public static string GetJsonPath(ConnectionPref pref)=> $@"{Directory.GetCurrentDirectory()}\{nameof(ConnectionPref)}.json";
    }
}