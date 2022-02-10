using ServerApp.Core;
using ServerTests.Modules;

namespace ServerTests.Configs;

public class TestServerAppCoreEntryPointCfg : ServerAppCoreEntryPointCfg
{
    public TestServerAppCoreEntryPointCfg()
    {
        base.SubModules.Add(new TestTcpClientModule());
    }
}