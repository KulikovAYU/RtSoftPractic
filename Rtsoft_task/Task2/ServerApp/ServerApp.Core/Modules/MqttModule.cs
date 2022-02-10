using Autofac;
using MQTTnet;
using MQTTnet.Server;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Mqtt;

namespace ServerApp.Core.Modules
{
    public class MqttModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //register mqtt MqttServer
            builder.RegisterType<MqttFactory>();
            builder.Register(c => c.Resolve<MqttFactory>().CreateMqttServer()).As<IMqttServer>();
            
            //register mqtt brocker with default settings
            builder.RegisterType<MqttBrocker>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), MqttBrocker.GetDefaultPrefs()));
            //or variant
            // builder.RegisterType<MqttBrocker>().
            //     WithParameter( new PositionalParameter(1, MqttBrocker.GetDefaultPrefs()));
        }
    }
}