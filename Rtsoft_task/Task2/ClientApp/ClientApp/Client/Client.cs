using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ClientApp.Base;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace ClientApp.Client
{
    public class Client : ViewModelBase
    {
        public bool IsConnected { get; private set; } = false;
        private StreamReader sReader_;
        private StreamWriter sWriter_;
        private TcpClient tcpClient_;
        private ConnectionPref pref_;
        private IMqttClient clientMqtt_;
        private readonly IEventBus eventBus_;

        public Client(IEventBus eventBus = null)
        {
            eventBus_ = eventBus;
            clientMqtt_ = new MqttFactory().CreateMqttClient();
        }

        public void SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                eventBus_?.Error("Incorrect JSON");
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
                    // ignored
                }
            });
        }


        void WaitForData()
        {
            Task.Factory.StartNew(async () =>
            {
                while (tcpClient_?.Connected == true)
                {
                    try
                    {
                        string sDataIncomming = await sReader_.ReadLineAsync();
                        eventBus_.OnResponse(sDataIncomming);

                        if (sDataIncomming == null)
                            return;

                    }
                    catch (Exception)
                    {
                        CloseConnection();
                    }
                }
            });
        }

        public async Task EstablishConnectionAsync(ConnectionPref pref)
        {
            tcpClient_ ??= new TcpClient();

            if (!tcpClient_.Connected)
            {
                try
                {
                    pref_ = pref;
                    eventBus_?.Print("Establishing connection");

                    await tcpClient_.ConnectAsync(pref_.HostNameOrAdress, pref_.PortNumber);

                    NetworkStream stream = tcpClient_.GetStream();
                    sReader_ = new StreamReader(stream, Encoding.ASCII);
                    sWriter_ = new StreamWriter(stream, Encoding.ASCII) {AutoFlush = true};

                    await sWriter_.WriteLineAsync(pref_.UserName);
                    await sWriter_.WriteLineAsync(pref_.Id.ToString());

                    await EstablishMqttConnectionAsync(pref_);

                    WaitForData();
                    IsConnected = tcpClient_.Connected;
                }
                catch (Exception ex)
                {
                    eventBus_?.Error(ex.Message);
                    IsConnected = false;
                }
            }
        }

        public void DisconnectCommand()
        {
            if (tcpClient_ == null)
                return;

            eventBus_?.Print("Start diconnection");
            NetworkStream networkStream = tcpClient_.GetStream();
            networkStream.Close();

            CloseConnection();
            DisconnectFromMqtt();
        }


        void CloseConnection()
        {
            tcpClient_.Close();
            tcpClient_ = null;
            IsConnected = false;
        }

        async Task EstablishMqttConnectionAsync(ConnectionPref pref)
        {
            //configure options
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithClientId(pref.Id.ToString())
                .WithTcpServer(pref.HostNameOrAdress, pref.PortNumber + 1)
                // .WithCredentials("bud", "%spencer%")
                .WithCleanSession()
                .Build();

            //Handlers
            clientMqtt_.UseConnectedHandler(async e =>
            {
                
                eventBus_?.Print($"Connected successfully with MQTT Brokers.");

                //Subscribe to topic
                await clientMqtt_.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("RemoteSrvrData/#").Build());
            });
            clientMqtt_.UseDisconnectedHandler(e =>
            {
                eventBus_?.Print("Disconnected from MQTT Brokers.");
                var reason = e.Reason;
            });
            clientMqtt_.UseApplicationMessageReceivedHandler(async e => { await eventBus_?.OnMqqtEvent(e); });

            //actually connect
            await clientMqtt_.ConnectAsync(options);
        }

        void DisconnectFromMqtt()
        {
            clientMqtt_.DisconnectAsync().Wait();
        }
    }
}