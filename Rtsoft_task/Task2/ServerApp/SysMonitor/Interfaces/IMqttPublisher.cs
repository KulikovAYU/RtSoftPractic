using SysMonitor.Devices;

namespace SysMonitor.Interfaces
{
    public interface IMqttPublisher
    {
        void Start();

        void Stop();

        public void AddDevice(IMqttMessageSender sender);
        public void RemoveDevice(DevidceType devType, string args = "");
    }
}