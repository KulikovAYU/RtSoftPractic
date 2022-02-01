using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SysMonitor.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SysMonitor.Mqtt
{
    public class MqttPublisher
    {
        private readonly SocketPrefs prefs_;
        private IMqttClient Client { get; set; }

        public MqttPublisher(SocketPrefs prefs)
        {
            prefs_ = prefs;
            Client = new MqttFactory().CreateMqttClient();
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
            //handlers
            Client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected successfully with MQTT Brokers.");
            });
            Client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });

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
                    .WithTcpServer(prefs_.IpAddress.ToString(), prefs_.PortNumber)
                    .WithCleanSession()
                    .Build();

                //connect
                Client.ConnectAsync(optionsBuilder).Wait();

                //Create new publish Task
                Task.Run(async () => {
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

        public void AddDevice(IMqqtMessageSender sender)
        {
            if(sender != null)
                devices_.Add(sender);
        }

        public void RemoveDeviceByTopic(string topicName)
        {
            devices_.Remove(devices_.FirstOrDefault(topName => topName.GetTopicName().Equals(topicName)));
        }
        
        public void RemoveDevice(DevidceType devType, string args = "")
        {
            devices_.Remove(devices_.FirstOrDefault(type => type.Type.Equals(devType) &&  args.Equals(type.GetServiceName())));
        }

        public void Stop()
        {
            Client.DisconnectAsync().Wait();
        }

        async Task StartPublishAsync()
        {
            foreach (var dev in devices_)
            {
                Console.WriteLine($"publishing at {DateTime.UtcNow}; client {dev.GetDescription()};");
                _ = await Client.PublishAsync(dev.GetMsg());
            }
              
        }

        private List<IMqqtMessageSender> devices_ = new List<IMqqtMessageSender>();
    }
}
