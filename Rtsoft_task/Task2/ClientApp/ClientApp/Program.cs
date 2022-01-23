using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientApp
{

    //class Client
    //{
    //    private TcpClient _tcpclient;

    //    private StreamReader _sReader;
    //    private StreamWriter _sWriter;
    //    public static List<string> lst_storeddata = new List<string>();

    //    private Boolean _isConnected;
    //    string name;
    //    string phone;
    //    string address;
    //    string passport;

    //    public object MessageBox { get; }

    //    public Client(string _name = "", string _phone = "", string _address = "", string _passport = "")
    //    {
    //        //server ip
    //        String ipAddress = "127.0.0.1";
    //        ipAddress = "localhost";
    //        //String ipAddress = "192.168.43.15";
    //        //port number
    //        int portNum = 11000;
    //        try
    //        {
    //            _tcpclient = new TcpClient();
    //            _tcpclient.Connect(ipAddress, portNum);

    //            name = _name;
    //            phone = _phone;
    //            address = _address;
    //            passport = _passport;

    //            HandleCommunication();
    //        }
    //        catch (Exception ex)
    //        {
    //           // MessageBox.Show(ex.Message);
    //        }
    //    }

    //    public void HandleCommunication()
    //    {
    //        _sReader = new StreamReader(_tcpclient.GetStream(), Encoding.ASCII);
    //        _sWriter = new StreamWriter(_tcpclient.GetStream(), Encoding.ASCII);

    //        string clientData = Environment.MachineName + " hello world";
    //        //string clientData = Environment.MachineName + "," + name + "," + phone + "," + address + "," + passport;
    //        _sWriter.WriteLine(clientData);
    //        _sWriter.Flush();

    //        // receive data
    //        string sDataIncomming = _sReader.ReadLine();
    //        //lst_storeddata = (sDataIncomming.Split(',')).ToList();
    //        //_sWriter.Close();
    //        _tcpclient.Close();
    //    }
    //}


    class Program
    {
        static void Main(string[] args)
        {
            //Client lci = new Client();

            ConnectionPref pref =
              ConnectionPref.NewBuilder().
              SetHostNameOrAdress("localhost").
              SetIp(Dns.GetHostEntry("localhost").AddressList[0]).
              SetPortNum(11000).
              Build();

            Client1 cli = new Client1(pref);
            cli.SendMessage("Hello world");

            Console.ReadLine();
        }
    }
}
