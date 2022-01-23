using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    public class TcpServer
    {
        private readonly IEventBus eventBus_;
        private bool isRunning_ = false;
        private TcpListener server_;
        private SocketPrefs pref_;

        public TcpServer(SocketPrefs pref, IEventBus eventBus = null)
        {
            pref_ = pref;
            eventBus_ = eventBus;
        }

        public void Start()
        {
            EstablishConnection();
            isRunning_ = true;

            //https://github.com/StepFanFly/CSoft-LogViewer/blob/dev/LogViewer/Infrastructure/Actors/AbstractActor.cs
            //https://stackoverflow.com/questions/46128734/passing-json-data-over-tcp-socket-programming-c-sharp
            Task.Run(async () => { await listenClientsAsync(); });
        }

        void EstablishConnection()
        {
            server_ = new TcpListener(pref_.IpAddress, pref_.PortNumber);
            server_.Start();
        }

        private async Task listenClientsAsync()
        {
            eventBus_?.Print($"Waiting for client on {pref_}");
            while (isRunning_)
            {
                try
                {
                    TcpClient newClient = await server_.AcceptTcpClientAsync();
                    eventBus_?.Print($"Client accepted {pref_}");

                    // client found.
                    // create a thread to handle communication
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(newClient);
                }
                catch (Exception ex)
                {
                    eventBus_?.Error($"Failed listening incomming connection: {ex}");
                }
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;

            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII) { AutoFlush = true };
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

            // reads from client stream
            string sData = sReader.ReadLine();

            eventBus_?.Print($"Recieved Data {sData}");

            sWriter.WriteLine("Hello from server =)");
            //sWriter.Flush();
            client.Close();
        }
    }
}