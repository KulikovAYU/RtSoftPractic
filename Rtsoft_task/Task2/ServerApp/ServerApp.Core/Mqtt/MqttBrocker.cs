using System;
using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;
using ServerApp.Core.TcpServer;

namespace ServerApp.Core.Mqtt
{
    /// <summary>
    /// Dispatcher between the senders and receivers
    /// </summary>
    public class MqttBrocker
    {
        private SocketPrefs prefs_;
        public static readonly IMqttServer Server;
        private IEventBus eventBus_;

        static MqttBrocker()
        {
            Server = new MqttFactory().CreateMqttServer();
        }

        public MqttBrocker(SocketPrefs prefs, IEventBus eventBus = null)
        {
            prefs_ = prefs;
            eventBus_ = eventBus;
        }

        public static SocketPrefs GetDefaultPrefs()
        {
           return
            SocketPrefs
            .NewBuilder()
            .SetIp("127.0.0.1")
            .SetMaxConnections(10)
            .SetPortNum(11001)
            .Build();
        }

        public void Start()
        {
            try
            {
                //https://www.codeproject.com/Articles/5283088/MQTT-Message-Queue-Telemetry-Transport-Protocol-wi
                //https://russianblogs.com/article/9111634963/
                //configure options
                var optionsBuilder = new MqttServerOptionsBuilder()
                    .WithConnectionValidator(c =>
                   {
                       eventBus_.Print($"{c.ClientId} connection validator for c.Endpoint: {c.Endpoint}");
                       c.ReasonCode = MqttConnectReasonCode.Success;
                   })
                    .WithConnectionBacklog(prefs_.MaxConnections)
                    .WithDefaultEndpointBoundIPAddress(prefs_.IpAddress)
                    .WithDefaultEndpointPort(prefs_.PortNumber)
                    .Build();

                //start server
                Server.StartAsync(optionsBuilder).Wait();

                eventBus_?.Print($"Mqqt Broker is Running. Configuration is {prefs_}");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                eventBus_?.Error(ex.Message);
            }
        }

        public void Stop()
        {
            eventBus_.Print("Stop broker");
            Server.StopAsync().Wait();
        }
    }
}