using MQTTnet;

namespace SysMonitor.Devices
{
    public interface IMqqtMessageSender
    {
        MqttApplicationMessage GetMsg();
        DevidceType Type { get;}
        string GetTopicName();
        string GetDescription();
    }
}
