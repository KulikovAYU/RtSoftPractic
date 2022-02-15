using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ClientApp.Utils
{

    public interface ISerrializer
    {
        string Serrialize<T>(T obj);
        T? DeSerrialize<T>(string data);
    }

    public class JsonSerrializer : ISerrializer
    {
        public string Serrialize<T>(T obj) => JsonSerializer.Serialize(obj);
       
        public T? DeSerrialize<T>(string data) => JsonSerializer.Deserialize<T>(data);
    }
}