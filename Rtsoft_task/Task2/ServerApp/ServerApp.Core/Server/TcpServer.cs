using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerApp.Core.Extentions;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server.Commands;

namespace ServerApp.Core.Server
{
    public class TcpServer : ITcpServer
    {
        private readonly IEventBus _eventBus;
        private bool _isRunning = false;
        private TcpListener _server;
        private readonly ISocketPrefs _pref;
        private readonly List<IConnectedClient> _clients = new List<IConnectedClient>(new List<ConnectedClient>());

        public TcpServer(ISocketPrefs pref, IEventBus eventBus = null)
        {
            _pref = pref;
            _eventBus = eventBus;
        }

        public static ISocketPrefs GetDefaultPrefs()
        {
            return
                  SocketPrefs
                  .NewBuilder()
                  .SetIp("127.0.0.1")
                  .SetMaxConnections(10)
                  .SetPortNum(11000)
                  .Build();
        }


        public void Start()
        {
            EstablishConnection();
            Task.Run(async () => { await ListenClientsAsync(); });
        }

        public void Stop()
        {
            _server.Stop();
            _isRunning = false;
        }

        public List<IConnectedClient> GetConnectedClients() =>_clients;
  
        void EstablishConnection()
        {
            _server = new TcpListener(_pref.IpAddress, _pref.PortNumber);
            _server.Start();
            _isRunning = true;
        }

        private async Task ListenClientsAsync()
        {
            _eventBus?.Print($"Waiting for client on {_pref}");

            while (_isRunning)
            {
                try
                {
                    //wait for client connection
                    TcpClient newClient = await _server.AcceptTcpClientAsync();


                    //create thread on connection
                    ConnectedClient client = new ConnectedClient(newClient);
                    _clients.Add(client);

                    _ = Task.Factory.StartNew(async () =>
                      {

                          using var sReader = new StreamReader(client.ClientData.GetStream(), Encoding.ASCII);
                          using var sWriter = new StreamWriter(client.ClientData.GetStream(), Encoding.ASCII) { AutoFlush = true };

                          client.Name = await sReader.ReadLineAsync();
                          string guid = await sReader.ReadLineAsync();
                          Guid.Parse(guid);

                          if (client.ClientData.Connected)
                          {
                              _eventBus?.Print($"Client {client.Name} has join at server!");
                              var test = new Response(CommandType.eEStablishConnect, 200, $"Hello {client.Name} from server =)").ToJson();
                              await sWriter.WriteLineAsync(new Response(CommandType.eEStablishConnect, 200, $"Hello {client.Name} from server =)").ToJson());
                          }

                          //ok. if we connected wait message from server
                          while (client.ClientData.Connected)
                          {
                              try
                              {
                                  // reads from client stream
                                  string sData = await sReader.ReadLineAsync();
                                  if (sData == null)
                                      break;

                                  _eventBus?.Print($"Recieved Data {sData}");

                                  //execute command
                                  var response = CommandExecutor.FromJson(sData);
                                  await sWriter.WriteLineAsync(response.ToJson());
                              }
                              catch (Exception)
                              {
                                  // ignored
                              }
                          }

                          _eventBus?.Print($"Client {client} has left from server");
                          client.ClientData.GetStream().Close();
                          client.ClientData.Close();
                          _clients.Remove(client);
                      });
                }
                catch (Exception ex)
                {
                    _eventBus?.Error($"Failed listening incomming connection: {ex}");
                }
            }
        }
    }
}