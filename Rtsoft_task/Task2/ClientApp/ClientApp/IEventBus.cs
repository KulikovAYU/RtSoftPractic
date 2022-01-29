using MQTTnet;

namespace ClientApp
{
    public interface IEventBus
    {
        void Print(string message);

        void Error(string message);

        void OnMqqtEvent(MqttApplicationMessageReceivedEventArgs args);

        void OnResponse(string message);
    }

  
}
