using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SysMonitor.Interfaces;


namespace SysMonitor.Mqtt
{
    public class MqttPublisher : IMqttPublisher
    {
        private readonly ISocketPrefs _prefs;
        private IMqttClient Client { get; set; }

        public MqttPublisher(IMqttClient mqttClient, ISocketPrefs prefs)
        {
            _prefs = prefs;

            Client = mqttClient;
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
            //handlers
            Client.UseConnectedHandler(e => { Console.WriteLine("Connected successfully with MQTT Brokers."); });
            Client.UseDisconnectedHandler(e => { Console.WriteLine("Disconnected from MQTT Brokers."); });

            Client.UseApplicationMessageReceivedHandler(e =>
            {
                try
                {
                    string topic = e.ApplicationMessage.Topic;
                    if (string.IsNullOrWhiteSpace(topic) == false)
                    {
                        string payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        Console.WriteLine($"Topic: {topic}. Message Received: {payload}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            });

            try
            {
                var optionsBuilder = new MqttClientOptionsBuilder()
                    .WithClientId($"{Guid.NewGuid()}")
                    .WithTcpServer(_prefs.IpAddress.ToString(), _prefs.PortNumber)
                    .WithCleanSession()
                    .Build();

                //connect
                Client.ConnectAsync(optionsBuilder).Wait();

                //Create new publish Task
                Task.Run(async () =>
                {
                    while (Client.IsConnected)
                    {
                        await StartPublishAsync();
                        Task.Delay(2000).Wait();
                    }
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void AddDevice(IMqttMessageSender sender)
        {
            if (sender != null)
                _devices.Add(sender);
        }

        public void RemoveDevice(DevidceType devType, string args = "")
        {
            _devices.Remove(_devices.FirstOrDefault(type =>
                type.Type.Equals(devType) && args.Equals(type.GetServiceName())));
        }

        public void Stop()
        {
            Client.DisconnectAsync().Wait();
        }

        async Task StartPublishAsync()
        {
            foreach (var dev in _devices)
            {
                Console.WriteLine($"publishing at {DateTime.UtcNow}; client {dev.GetDescription()}");
                _ = await Client.PublishAsync(dev.GetMsg());
            }
        }

        private readonly List<IMqttMessageSender> _devices = new List<IMqttMessageSender>();
    }
}