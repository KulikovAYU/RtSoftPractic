using System.Linq;
using Autofac;
using MQTTnet;
using SysMonitor.Devices;
using SysMonitor.Interfaces;
using SysMonitor.Mqtt;
using MQTTnet.Client;

namespace SysMonitor
{
    public class SysMonitorModule : Module
    {
        public static IContainer Ioc { get;  set; }
        
        protected override void Load(ContainerBuilder builder)
        {
            //register MQTT devices pool
            builder.RegisterType<SysMonitorsPool>()
                .As<ISysMonitorsPool>()
                .SingleInstance();
                
            //declare Mqtt client
            builder.RegisterType<MqttFactory>().AsSelf();
            builder.Register(c => c.Resolve<MqttFactory>().CreateMqttClient())
                .As<IMqttClient>();

            //register Mqtt MqttPublisher
            builder.RegisterType<MqttPublisher>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), MqttPublisher.GetDefaultPrefs()))
                .As<IMqttPublisher>();

            //register devices
            builder.RegisterType<MqttCpuTemperatureMonitor>().SingleInstance();
            builder.Register<IMqttMessageSender>((c, p) =>
            {
                var parameters = p.ToList();
                if (parameters.Count == 0)
                    return c.Resolve<MqttCpuTemperatureMonitor>();

                return new MqttCpuServiceMonitor(parameters.Named<int>("procId"), parameters.Named<string>("serviceName"));
            });
           
            
            //register Mqtt device factory
            builder.RegisterType<DeviceFactory>().AsSelf().SingleInstance();
            
           // builder.
        }
    }

    /// <summary>
    /// New entry point
    /// </summary>
    public static class SysMonitorEntryPointCfgR
    {
        public static IContainer Ioc { get; private set; }

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            
            //register MQTT devices pool
            builder.RegisterType<SysMonitorsPool>()
                .As<ISysMonitorsPool>()
                .SingleInstance();
                
            //declare Mqtt client
            builder.RegisterType<MqttFactory>().AsSelf();
            builder.Register(c => c.Resolve<MqttFactory>().CreateMqttClient())
                    .As<IMqttClient>();

            //register Mqtt MqttPublisher
            builder.RegisterType<MqttPublisher>()
                    .WithParameter(new TypedParameter(typeof(ISocketPrefs), MqttPublisher.GetDefaultPrefs()))
                    .As<IMqttPublisher>();

            //register devices
            builder.RegisterType<MqttCpuTemperatureMonitor>().SingleInstance();
            builder.Register<IMqttMessageSender>((c, p) =>
            {
                var parameters = p.ToList();
                if (parameters.Count == 0)
                    return c.Resolve<MqttCpuTemperatureMonitor>();

                return new MqttCpuServiceMonitor(parameters.Named<int>("procId"), parameters.Named<string>("serviceName"));
            });
           
            
            //register Mqtt device factory
            builder.RegisterType<DeviceFactory>().AsSelf().SingleInstance();
            
            Ioc = builder.Build();

            return Ioc;
        }
    }
}