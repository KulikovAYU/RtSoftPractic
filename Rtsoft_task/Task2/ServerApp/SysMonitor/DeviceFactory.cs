using Autofac;
using SysMonitor.Interfaces;

namespace SysMonitor
{
    public class DeviceFactory
    {
        public IMqttMessageSender Create(DevidceType type, string args = "")
        {
            switch (type)
            {
                case DevidceType.eCPUMonitor:
                {
                    if (Utils.GetProcIdByServiceName(out var procId, args))
                        return SysMonitorModule.Ioc.Resolve<IMqttMessageSender>(
                            new NamedParameter("procId", procId),
                            new NamedParameter("serviceName", args));
                    break;
                }
                case DevidceType.eCPUTemp:
                {
                    return SysMonitorModule.Ioc.Resolve<IMqttMessageSender>();
                }
                case DevidceType.eUndef:
                default:
                    break;
            }

            return null;
        }
    }
}