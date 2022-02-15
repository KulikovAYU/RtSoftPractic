using System.Threading.Tasks;
using MQTTnet;

namespace ClientApp.Base
{
    public interface IEventBus
    {
        void Print(string message);

        void Error(string message);

        Task OnMqqtEvent(MqttApplicationMessageReceivedEventArgs args);

        void OnResponse(string? message);
    }
}