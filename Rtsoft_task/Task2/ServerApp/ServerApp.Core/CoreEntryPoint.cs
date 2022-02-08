using ServerApp.Core.Interfaces;
using ServerApp.Core.Mqtt;
using SysMonitor.Interfaces;

namespace ServerApp.Core
{
    class ServerApplication : IServerApplication
    {
        private readonly ITcpServer _tcpServer;
        private readonly ISysMonitorsPool _sysMonitorsPool;
        private readonly MqttBrocker _mqttBrocker;

        public ServerApplication(ITcpServer tcpServer, ISysMonitorsPool sysMonitorsPool, MqttBrocker mqttBrocker)
        {
            _tcpServer = tcpServer;
            _sysMonitorsPool = sysMonitorsPool;
            _mqttBrocker = mqttBrocker;
        }

        public void Run()
        {
            _tcpServer.Start();
            _mqttBrocker.Start();
            
            //MQTT publisher starting
            _sysMonitorsPool.StartServices();
        }

        public void Stop()
        {
            _tcpServer.Stop();
            _mqttBrocker.Stop();
            
            //MQTT publisher stopping
            _sysMonitorsPool.StopServices();
        }
    }
}