using Autofac;
using SysMonitor.Interfaces;

namespace SysMonitor
{
    public enum DevidceType {eUndef, eCPUMonitor, eCPUTemp }

    public class SysMonitorsPool : ISysMonitorsPool
    {
        private static IMqttPublisher _mqttPublisher;
        
        public SysMonitorsPool(IMqttPublisher mqttPublisher)
        {
            _mqttPublisher = mqttPublisher;
        }
        
        public void StartServices()
        {
            _mqttPublisher.Start();
            CreateDevice(DevidceType.eCPUTemp);
        }

        public void StopServices()
        {
            _mqttPublisher.Stop();
        }

        public static void CreateDevice(DevidceType type,string args = null)
        {
            var device = SysMonitorModule.Ioc.Resolve<DeviceFactory>().Create(type, args);
            if(device != null)
                _mqttPublisher.AddDevice(device);
        }

        public static void RemoveDevice(DevidceType type, string args = null)
        {
            _mqttPublisher.RemoveDevice(type, args);
            //TODO: Impl
        }
    }
}