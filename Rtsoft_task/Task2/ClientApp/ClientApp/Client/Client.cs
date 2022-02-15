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
        public bool IsConnected { get; private set; }
        private StreamReader _sReader;
        private StreamWriter _sWriter;
        private TcpClient _tcpClient;
        private ConnectionPref _pref;
        private MqttConnectionPref _mqttPref;
        private readonly IMqttClient _clientMqtt;
        private readonly IEventBus? _eventBus;

        public Client(IEventBus? eventBus = null)
        {
            _eventBus = eventBus;
            _clientMqtt = new MqttFactory().CreateMqttClient();
        }

        public void SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                _eventBus?.Error("Incorrect JSON");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _eventBus?.Print($"Sending JSON {msg}");
                    _sWriter.WriteLine(msg);
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
                while (_tcpClient?.Connected == true)
                {
                    try
                    {
                        string? sDataIncomming = await _sReader.ReadLineAsync();
                        _eventBus.OnResponse(sDataIncomming);

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

        public async Task EstablishConnectionAsync(ConnectionPref pref, MqttConnectionPref mqttPref)
        {
            _tcpClient ??= new TcpClient();

            if (!_tcpClient.Connected)
            {
                try
                {
                    _pref = pref;
                    _mqttPref = mqttPref;
                    _eventBus?.Print("Establishing connection");

                    await _tcpClient.ConnectAsync(_pref.HostNameOrAdress, _pref.PortNumber);

                    NetworkStream stream = _tcpClient.GetStream();
                    _sReader = new StreamReader(stream, Encoding.ASCII);
                    _sWriter = new StreamWriter(stream, Encoding.ASCII) {AutoFlush = true};

                    await _sWriter.WriteLineAsync(_pref.UserName);
                    await _sWriter.WriteLineAsync(_pref.Id.ToString());

                    await EstablishMqttConnectionAsync(_pref);

                    WaitForData();
                    IsConnected = _tcpClient.Connected;
                }
                catch (Exception ex)
                {
                    _eventBus?.Error(ex.Message);
                    IsConnected = false;
                }
            }
        }

        public void DisconnectCommand()
        {
            if (_tcpClient == null)
                return;

            _eventBus?.Print("Start diconnection");
            NetworkStream networkStream = _tcpClient.GetStream();
            networkStream.Close();

            CloseConnection();
            DisconnectFromMqtt();
        }

        void CloseConnection()
        {
            _tcpClient.Close();
            _tcpClient = null;
            IsConnected = false;
        }

        async Task EstablishMqttConnectionAsync(ConnectionPref pref)
        {
            //configure options
            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithClientId(_mqttPref.Id.ToString())
                .WithTcpServer(_mqttPref.HostAdress, _mqttPref.PortNumber)
                // .WithCredentials("bud", "%spencer%")
                .WithCleanSession()
                .Build();

            //Handlers
            _clientMqtt.UseConnectedHandler(async e =>
            {
                _eventBus?.Print($"Connected successfully with MQTT Brokers.");

                //Subscribe to topic
                await _clientMqtt.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("RemoteSrvrData/#").Build());
            });
            _clientMqtt.UseDisconnectedHandler(e =>
            {
                _eventBus?.Print($"Disconnected from MQTT Brokers. Reason:{e.Reason.ToString()}");
            });
            _clientMqtt.UseApplicationMessageReceivedHandler(async e => { await _eventBus?.OnMqqtEvent(e); });

            //actually connect
            await _clientMqtt.ConnectAsync(options);
        }

        void DisconnectFromMqtt()
        {
            _clientMqtt.DisconnectAsync().Wait();
        }
    }
}