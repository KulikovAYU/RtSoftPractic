using System;
using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Protocol;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server;

namespace ServerApp.Core.Mqtt
{
    /// <summary>
    /// Dispatcher between the senders and receivers
    /// </summary>
    class MqttBrocker
    {
        private readonly ISocketPrefs _prefs;
        private readonly IMqttServer _server;
        private readonly IEventBus _eventBus;

        public MqttBrocker(IMqttServer server, ISocketPrefs prefs, IEventBus eventBus = null)
        {
            _server = server;
            _prefs = prefs;
            _eventBus = eventBus;
        }

        public static ISocketPrefs GetDefaultPrefs()
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
                       _eventBus.Print($"{c.ClientId} connection validator for c.Endpoint: {c.Endpoint}");
                       c.ReasonCode = MqttConnectReasonCode.Success;
                   })
                    .WithConnectionBacklog(_prefs.MaxConnections)
                    .WithDefaultEndpointBoundIPAddress(_prefs.IpAddress)
                    .WithDefaultEndpointPort(_prefs.PortNumber)
                    .Build();

                //start server
                _server.StartAsync(optionsBuilder).Wait();

                _eventBus?.Print($"Mqqt Broker is Running. Configuration is {_prefs}");
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                _eventBus?.Error(ex.Message);
            }
        }

        public void Stop()
        {
            _eventBus.Print("Stop broker");
            _server.StopAsync().Wait();
        }
    }
}