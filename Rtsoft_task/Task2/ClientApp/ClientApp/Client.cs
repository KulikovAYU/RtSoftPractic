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


        public Client1(ConnectionPref pref)
        {
            pref_ = pref;
            EstablishConnection();
            WaitForData();
        }

        public void SendMessage(string msg)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
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
                            //Todo Send Event to bus

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

        void EstablishConnection()
        {
            client_ = new TcpClient();
            client_.Connect(pref_.IpAddress, pref_.PortNumber);

            NetworkStream stream = client_.GetStream();
            sReader_ = new StreamReader(stream, Encoding.ASCII);
            sWriter_ = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
        }
    }
}
