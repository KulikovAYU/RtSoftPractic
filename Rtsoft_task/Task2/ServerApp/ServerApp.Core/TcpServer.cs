using ServerApp.Core.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    public class TcpServer
    {
        private readonly IEventBus eventBus_;
        private bool isRunning_ = false;
        private TcpListener server_;
        private SocketPrefs pref_;
        private List<ConnectedClient> clients_ = new List<ConnectedClient>();

        public TcpServer(SocketPrefs pref, IEventBus eventBus = null)
        {
            pref_ = pref;
            eventBus_ = eventBus;
        }

        public void Start()
        {
            EstablishConnection();
            Task.Run(() => { CheckDisconnectedClients(); });
            Task.Run(async () => { await listenClientsAsync(); });
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

        private async Task listenClientsAsync()
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

                          StreamReader sReader = new StreamReader(client.ClientData.GetStream(), Encoding.ASCII);
                          StreamWriter sWriter = new StreamWriter(client.ClientData.GetStream(), Encoding.ASCII) { AutoFlush = true };

                          client.Name = await sReader.ReadLineAsync();
                          if (client.ClientData.Connected)
                          {
                              eventBus_?.Print($"Client {client.Name} has join at server!");
                              await sWriter.WriteLineAsync($"Hello {client.Name} from server =)");
                          }

                          //ok. if we connected wait message from server
                          while (client.ClientData.Connected)
                          {
                              //send back response

                              // reads from client stream
                              string sData = await sReader.ReadLineAsync();
                              if (sData == null)
                                  break;

                              eventBus_?.Print($"Recieved Data {sData}");

                              //execute command
                              if (CommandExecutor.FromJSON(sData))
                              {
                                  eventBus_?.Print($"Command executed sucsesfully by client {client.Name} !");
                                  await sWriter.WriteLineAsync($"Command executed successfully");
                              }
                              else 
                              {
                                  eventBus_?.Print($"Command executed failed by client {client.Name} !");
                                  await sWriter.WriteLineAsync($"Command executed failed");
                              }
                          }

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

        private void CheckDisconnectedClients()
        {
            //waits and remove clients which state are disconnected
            while (isRunning_)
            {
                Task.Delay(1000).Wait();
                if (clients_.Count != 0)
                {
                    clients_.RemoveAll((cli) =>
                    {
                        if (!cli.ClientData.Connected)
                        {
                            eventBus_?.Print($"Client {cli} has left from server");
                            return true;
                        }

                        return false;
                    });
                }
            }
        }
    }
}