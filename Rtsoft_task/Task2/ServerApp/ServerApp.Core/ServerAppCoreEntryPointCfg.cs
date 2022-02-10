using System.Collections.Generic;
using Autofac.Core;
using ServerApp.Core.Modules;
using SysMonitor;

namespace ServerApp.Core
{
    public class ServerAppCoreEntryPointCfg : ContainerOfModulesBase
    {
        protected override List<IModule> SubModules { get; set; } = new(){
                new ServerModule(),
                new CommandsModule(),
                new SysMonitorModule(),
                new MqttModule()
                //Add submodules here
        };
    }
}