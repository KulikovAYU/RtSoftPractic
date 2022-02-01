using ServerApp.Core.Mqtt;
using ServerApp.Core.TcpServer;
using SysMonitor;

namespace ServerApp.Core
{
    public static class CoreEntryPoint
    {
        public static TcpServer.TcpServer commandTcpSrv { get; private set; }
        public static MqttBrocker mqqtBrocker { get; private set; }



        public static void StartServices(IEventBus eventBus = null)
        {

            #region TCP Server configuration

            commandTcpSrv = new TcpServer.TcpServer(TcpServer.TcpServer.GetDefaultPrefs(), eventBus);
            commandTcpSrv.Start();

            #endregion


            #region MQTT brocker configuration
            mqqtBrocker = new MqttBrocker(MqttBrocker.GetDefaultPrefs(), eventBus);
            mqqtBrocker.Start();
            #endregion


            #region MQTT publisher configuration

            SysMonitorsPool.StartServices();

            #endregion
        }

        public static void StopServices(IEventBus eventBus = null)
        {
            commandTcpSrv.Stop();
            mqqtBrocker.Stop();
            SysMonitorsPool.Stop();
        }
    }
}
