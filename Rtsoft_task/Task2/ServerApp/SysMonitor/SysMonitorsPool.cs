using SysMonitor.Devices;
using SysMonitor.Mqtt;

namespace SysMonitor
{
    public enum DevidceType {eUndef, eCPUMonitor, eCPUTemp }

    public static class SysMonitorsPool
    {
        private static MqttPublisher MqqtPublisher { get; set; }

        public static void StartServices()
        {
            #region MQTT publisher configuration

            MqqtPublisher = new MqttPublisher(MqttPublisher.GetDefaultPrefs());
            MqqtPublisher.Start();

            #endregion

            CreateDevice(DevidceType.eCPUTemp);
        }

        public static void Stop()
        {
            MqqtPublisher.Stop();
        }

        public static void CreateDevice(DevidceType type,string args = null)
        {
            switch (type)
            {
                case DevidceType.eCPUMonitor:
                    {
                        if (Utils.GetProcIdByServiceName(out var procId, args))
                            MqqtPublisher.AddDevice(new MqqtCpuServiceMonitor(procId, args));
                        break;
                    }
                case DevidceType.eCPUTemp:
                    {
                        MqqtPublisher.AddDevice(new MqqtCpuTemperatureMonitor());
                        break;
                    }
                case DevidceType.eUndef:
                default:
                    break;
            }
        }

        public static void RemoveDevice(DevidceType type, string args = null)
        {
            MqqtPublisher.RemoveDevice(type, args);
            //MqqtPublisher.RemoveDevice()
            //TODO: Impl

        }
    }
}