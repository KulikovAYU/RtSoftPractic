using Autofac;
using MQTTnet;
using MQTTnet.Server;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Mqtt;
using ServerApp.Core.Server;
using SysMonitor;
using ISocketPrefs = ServerApp.Core.Interfaces.ISocketPrefs;

namespace ServerApp.Core
{
    /// <summary>
    /// New entry point
    /// </summary>
    public static class ServerAppCoreEntryPointCfg
    {
        public static IContainer Ioc { get; private set; }

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            //register entry point of that application
            builder.RegisterType<ServerApplication>().As<IServerApplication>();
            
            
            //register event bus
            builder.RegisterType<EventBusImpl>().As<IEventBus>().SingleInstance();
            
            
            //register tcp server with default settings
            builder.RegisterType<TcpServer>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), TcpServer.GetDefaultPrefs())).As<ITcpServer>();
            
            //register mqtt brocker with default settings
            builder.RegisterType<MqttBrocker>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), MqttBrocker.GetDefaultPrefs())).AsSelf();
            //or variant
            // builder.RegisterType<MqttBrocker>().
            //     WithParameter( new PositionalParameter(1, MqttBrocker.GetDefaultPrefs())).AsSelf();
            
            
            //register mqtt MqttServer
            builder.RegisterType<MqttFactory>().AsSelf();
            builder.Register(c => c.Resolve<MqttFactory>().CreateMqttServer()).As<IMqttServer>();
            
            builder.RegisterType<MqttBrocker>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), MqttBrocker.GetDefaultPrefs())).AsSelf();
            
            builder.RegisterModule<SysMonitorModule>();
            
            Ioc = builder.Build();
            SysMonitorModule.Ioc = Ioc;
            
            return Ioc;
        }
    }
}