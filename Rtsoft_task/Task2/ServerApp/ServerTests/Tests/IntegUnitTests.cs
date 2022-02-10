using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Autofac;
using ServerApp.Core.Extentions;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server;
using ServerApp.Core.Server.Commands;
using ServerTests.Configs;
using Xunit;

namespace ServerTests.Tests;

public class IntegUnitTests : IClassFixture<TestServerAppCoreEntryPointCfg>, IDisposable
{
    private readonly ITcpServer _tcpServer;
    private readonly TcpClient _tcpClient;
    
    private readonly StreamReader _sReader;
    private readonly StreamWriter _sWriter;

    public IntegUnitTests(TestServerAppCoreEntryPointCfg fixture)
    {
        //preparing and establishing TCP connection
        ILifetimeScope container = fixture.Configure();
        _tcpServer = container.Resolve<ITcpServer>();
        _tcpClient = container.Resolve<TcpClient>();

        var prefs = container.Resolve<ISocketPrefs>();

        _tcpServer.Start();

        _tcpClient.Connect(prefs.IpAddress, prefs.PortNumber);

        var isConnected = _tcpClient.Connected;

        var clientStream = _tcpClient.GetStream();
        _sWriter = new StreamWriter(clientStream, Encoding.ASCII) {AutoFlush = true};
        _sReader = new StreamReader(clientStream, Encoding.ASCII);

        _sWriter.WriteLine("TestUser");
        _sWriter.WriteLine(Guid.NewGuid().ToString());

        _sReader.ReadLine();
    }
    
    [Fact]
    public void RunRemoteDbus()
    {
        string serviceName = "foo-daemon.service";
        bool expectedResult = true;
        
        var remoteCmd = new ClientCommand(CommandType.eRunDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, -1).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == 200, expectedResult);
    }

    [Fact]
    public void StopRemoteDbus()
    {
        string serviceName = "foo-daemon.service";
        bool expectedResult = true;
        
        var remoteCmd = new ClientCommand(CommandType.eStopDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, -1).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == 200, expectedResult);
    }

    public void Dispose()
    {
        _tcpClient.GetStream().Close();
        _tcpClient.Dispose();
        
        _tcpServer.Stop();
    }
}