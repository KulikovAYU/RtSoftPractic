using System.Collections.Generic;
using Autofac.Core;
using ServerApp.Core.Modules;
using ServerTests.Modules;

namespace ServerTests.Configs;

public class StubTestServerCfg : ContainerOfModulesBase
{
    protected override List<IModule> SubModules { get; set; } = new()
    {
        new ServerModule(),
        new TestTcpClientModule(),
        new TestCommandModule()
        //Add submodules here
    };
}