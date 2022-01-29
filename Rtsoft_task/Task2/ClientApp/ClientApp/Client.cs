using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using ProtoBuf;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Client
    {
        private StreamReader sReader_;
        private StreamWriter sWriter_;
        private TcpClient client_;
        private ConnectionPref pref_;
        private IMqttClient clientMqtt_;
        IEventBus eventBus_;

        public Client(IEventBus eventBus = null)
        {
            eventBus_ = eventBus;
            //EstablishConnection();
        }

        public void SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                eventBus_?.Error("Icorrect JSON");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    eventBus_.Print($"Sending JSON {msg}");
                    sWriter_.WriteLine(msg);
                }
                catch (Exception)
                {
                }
            });
        }

        void WaitForData()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (client_?.Connected == true)
                        {
                            // string sDataIncomming = sReader_.ReadLine();
                            string sDataIncomming = await sReader_.ReadLineAsync();

                            eventBus_.OnResponse(sDataIncomming);

                            if (sDataIncomming == null)
                                client_.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }

                    Task.Delay(10).Wait();
                }
            });
        }

        public void EstablishConnection(ConnectionPref pref)
        {
            pref_ = pref;
            eventBus_?.Print("Установка соединения");
            client_ = new TcpClient();
            client_.Connect(pref_.HostNameOrAdress, pref_.PortNumber);
            eventBus_?.Print("Соединение установлено");

            NetworkStream stream = client_.GetStream();
            sReader_ = new StreamReader(stream, Encoding.ASCII);
            sWriter_ = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            
            sWriter_.WriteLine(pref_.UserName);
            sWriter_.WriteLine(pref_.Id);

            EstablishMqttConnection(pref_);

            WaitForData();
        }

        void EstablishMqttConnection(ConnectionPref pref)
        {
            clientMqtt_ = new MqttFactory().CreateMqttClient();

            //configure options
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                  .WithClientId(pref.Id.ToString())
                  .WithTcpServer(pref.HostNameOrAdress, pref.PortNumber + 1)
                  //.WithCredentials("bud", "%spencer%")
                  .WithCleanSession()
                  .Build();

            //Handlers
            clientMqtt_.UseConnectedHandler(e =>
            {
                eventBus_?.Print("Connected successfully with MQTT Brokers.");
                //Console.WriteLine("Connected successfully with MQTT Brokers.");

                //Subscribe to topic
                clientMqtt_.SubscribeAsync(new TopicFilterBuilder().WithTopic("RemoteSrvrData/#").Build()).Wait();

            });
            clientMqtt_.UseDisconnectedHandler(e =>
            {
                eventBus_?.Print("Disconnected from MQTT Brokers.");
                //Console.WriteLine("Disconnected from MQTT Brokers.");
            });
            clientMqtt_.UseApplicationMessageReceivedHandler(e =>
            {
                eventBus_?.OnMqqtEvent(e);
              
            });

            //actually connect
            clientMqtt_.ConnectAsync(options).Wait();
        }

    }
}
