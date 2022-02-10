using Autofac;
using ServerApp.Core.Server.Commands;

namespace ServerApp.Core.Modules
{
    public class CommandsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //register commands
            builder.RegisterType<RemoteRunProcCmd>();
            builder.RegisterType<RemoteStopProcCmd>();
            builder.RegisterType<RemoteRunDbusCmd>();
            builder.RegisterType<RemoteStopDbusCmd>();
            builder.RegisterType<RemoteRunTmdsDbusCmd>();
            builder.RegisterType<RemoteStopTmdsDbusCmd>();
        }
    }
}