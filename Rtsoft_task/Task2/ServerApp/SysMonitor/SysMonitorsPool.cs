using SysMonitor.Devices;
using SysMonitor.Mqtt;

namespace SysMonitor
{
    public enum DevidceType {eUndef, eCPUMonitor, eCPUTemp }

    public class SysMonitorsPool
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

        public static void CreateDevice(DevidceType type,string Args = null)
        {
            switch (type)
            {
                case DevidceType.eCPUMonitor:
                    {
                        int procId;
                        if (Utils.GetProcIdByServiceName(out procId, Args))
                            MqqtPublisher.AddDevice(new MqqtCPUServiceMonitor(procId, Args));
                        break;
                    }
                case DevidceType.eCPUTemp:
                    {
                        MqqtPublisher.AddDevice(new MqqtCPUTemperatureMonitor());
                        break;
                    }
                case DevidceType.eUndef:
                default:
                    break;
            }
        }

        public static void RemoveDevice(DevidceType type, string Args = null)
        {
           
            //MqqtPublisher.RemoveDevice()
            //TODO: Impl

        }
    }
}