using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Client1
    {
        private StreamReader sReader_;
        private StreamWriter sWriter_;
        private TcpClient client_;
        private ConnectionPref pref_;
        IEventBus eventBus_;

        public Client1(IEventBus eventBus = null)
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
                catch (Exception ex)
                {
                }
            });
        }

        void WaitForData()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (client_?.Connected == true)
                        {
                            string sDataIncomming = sReader_.ReadLine();
                            eventBus_?.Print($"Получены данные от сервера {sDataIncomming}");
                            //Todo Send Event to bus
                            //if (message != null)
                            //    message = sDataIncomming;

                            if (sDataIncomming == null)
                            {
                                client_.Close();
                            }
                        }
                    }
                    catch (Exception ex)
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

            WaitForData();
        }

    }
}
