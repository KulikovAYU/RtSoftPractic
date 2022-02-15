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

[Collection("Stub object Test Collection")]
[CollectionDefinition("Stub object Test Collection",DisableParallelization = true)]
/// <summary>
/// test with stub command objects
/// </summary>
public class StubUnitTests : IClassFixture<StubTestServerCfg>, IDisposable
{
    private readonly ITcpServer _tcpServer;
    private readonly TcpClient _tcpClient;
    
    private readonly StreamReader _sReader;
    private readonly StreamWriter _sWriter;

    //before running unit tests
    public StubUnitTests(StubTestServerCfg fixture)
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
    public void RunRemoteDbusPositiveResult()
    {
        string serviceName = "foo-daemon.service";
        bool expectedResult = true;
        
        var remoteCmd = new ClientCommand(CommandType.eRunDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, -1).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == StatCodes.SUCSESS, expectedResult);
    }
    
    [Fact]
    public void RunRemoteDbusNegativeResult()
    {
        string serviceName = "negative-foo-daemon.service";
        bool expectedResult = false;
        
        var remoteCmd = new ClientCommand(CommandType.eRunDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, -1).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == StatCodes.SUCSESS, expectedResult);
    }
    
    [Fact]
    public void StopRemoteDbusPositiveResult()
    {
        string serviceName = "foo-daemon.service";
        bool expectedResult = true;
        
        var remoteCmd = new ClientCommand(CommandType.eStopDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, -1).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == StatCodes.SUCSESS, expectedResult);
    }
    
    [Fact]
    public void StopRemoteDbusNegativeResult()
    {
        string serviceName = "negative-foo-daemon.service";
        bool expectedResult = false;
        
        var remoteCmd = new ClientCommand(CommandType.eStopDbus) {Guid = Guid.NewGuid(), Name = serviceName};

        string runRemoteProcCmd = remoteCmd.ToJson();
        _sWriter.WriteLine(runRemoteProcCmd);

        string? sDataIncomming = _sReader.ReadLine();

        var resp = new CommandResponse(Guid.Empty, CommandType.eUndef, StatCodes.UNDEF).FromJson(sDataIncomming);

        Assert.Equal(resp?.StatusCode == StatCodes.SUCSESS, expectedResult);
    }

    public void Dispose()
    {
        _tcpClient.GetStream().Close();
        _tcpClient.Dispose();
        
        _tcpServer.Stop();
    }
}