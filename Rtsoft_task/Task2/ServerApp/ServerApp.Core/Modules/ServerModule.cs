using Autofac;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server;

namespace ServerApp.Core.Modules
{
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //register entry point of that application
            builder.RegisterType<ServerApplication>().As<IServerApplication>();
            
            
            //register event bus
            builder.RegisterType<EventBusImpl>().As<IEventBus>().SingleInstance();
            
            
            //register tcp server with default settings
            builder.RegisterType<TcpServer>()
                .WithParameter(new TypedParameter(typeof(ISocketPrefs), TcpServer.GetDefaultPrefs())).As<ITcpServer>();
        }
    }
}