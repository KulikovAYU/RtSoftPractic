using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server.Commands;

namespace ServerApp.Core.Server
{
    public class TcpServer : ITcpServer
    {
        private readonly IEventBus eventBus_;
        private bool isRunning_ = false;
        private TcpListener server_;
        private ISocketPrefs pref_;
        private List<ConnectedClient> clients_ = new List<ConnectedClient>();

        public TcpServer(ISocketPrefs pref, IEventBus eventBus = null)
        {
            pref_ = pref;
            eventBus_ = eventBus;
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
            isRunning_ = false;
        }

        void EstablishConnection()
        {
            server_ = new TcpListener(pref_.IpAddress, pref_.PortNumber);
            server_.Start();
            isRunning_ = true;
        }

        private async Task ListenClientsAsync()
        {
            eventBus_?.Print($"Waiting for client on {pref_}");

            while (isRunning_)
            {
                try
                {
                    //wait for client connection
                    TcpClient newClient = await server_.AcceptTcpClientAsync();


                    //create thread on connection
                    ConnectedClient client = new ConnectedClient(newClient);
                    clients_.Add(client);

                    _ = Task.Factory.StartNew(async () =>
                      {

                          using var sReader = new StreamReader(client.ClientData.GetStream(), Encoding.ASCII);
                          using var sWriter = new StreamWriter(client.ClientData.GetStream(), Encoding.ASCII) { AutoFlush = true };

                          client.Name = await sReader.ReadLineAsync();
                          string guid = await sReader.ReadLineAsync();
                          Guid.Parse(guid);

                          if (client.ClientData.Connected)
                          {
                              eventBus_?.Print($"Client {client.Name} has join at server!");
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

                                  eventBus_?.Print($"Recieved Data {sData}");

                                  //execute command
                                  var response = CommandExecutor.FromJson(sData);
                                  await sWriter.WriteLineAsync(response.ToJson());
                              }
                              catch (Exception)
                              {
                                  // ignored
                              }
                          }

                          eventBus_?.Print($"Client {client} has left from server");
                          client.ClientData.GetStream().Close();
                          client.ClientData.Close();
                          clients_.Remove(client);
                      });
                }
                catch (Exception ex)
                {
                    eventBus_?.Error($"Failed listening incomming connection: {ex}");
                }
            }
        }
    }
}