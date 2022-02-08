using MQTTnet;

namespace SysMonitor.Interfaces
{
    public interface IMqttMessageSender
    {
        MqttApplicationMessage GetMsg();
        DevidceType Type { get;}
        string GetTopicName();
        string GetDescription();
        string GetServiceName();
    }
}
