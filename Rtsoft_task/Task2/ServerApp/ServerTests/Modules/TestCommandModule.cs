using System;
using System.Linq;
using Autofac;
using Moq;
using ServerApp.Core.Server;
using ServerApp.Core.Server.Commands;

namespace ServerTests.Modules;

class TestCommandModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        //register stub command objects
        builder.Register((c, p) =>
        {
            string reqProcName = "foo-daemon.service";
            var parameters = p.ToList();
            
            var runProcMock = new Mock<RemoteRunProcCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runProcMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunProc);
            runProcMock.Setup(m => m.Execute())
                .Returns((RemoteRunProcCmd cmd)=>(cmd.Name.Equals(reqProcName) ? 
                    new Response(CommandType.eRunProc, StatCodes.SUCSESS, "Ok") :
                    new Response(CommandType.eRunProc, StatCodes.NO_CONTENT, "Failed")));

            return runProcMock.Object;
        }).As<RemoteRunProcCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            
            var stopProcMock = new Mock<RemoteStopProcCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopProcMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopProc);
            stopProcMock.Setup(m => m.Execute())
                .Returns(new Response(CommandType.eStopProc, StatCodes.SUCSESS, "Ok"));
            
            return stopProcMock.Object;
        }).As<RemoteStopProcCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            
            var runDbusMock = new Mock<RemoteRunDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runDbusMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunDbus);
            runDbusMock.Setup(m => m.Execute())
                .Returns(new Response(CommandType.eRunDbus, StatCodes.SUCSESS, "Ok"));
            
            return runDbusMock.Object;
        }).As<RemoteRunDbusCmd>();

        builder.Register((c, p) =>
        {
            string reqProcName = "foo-daemon.service";
            
            var parameters = p.ToList();

            if (parameters.Positional<string>(1).Equals(reqProcName))
            {
                var runTmdsDbusPositiveMock = new Mock<RemoteRunTmdsDbusCmd>(parameters.Positional<Guid>(0),
                    parameters.Positional<string>(1), parameters.Positional<string>(2));
                runTmdsDbusPositiveMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunDbus);
                runTmdsDbusPositiveMock.Setup(m => m.Execute())
                    .Returns(new Response(CommandType.eRunDbus, StatCodes.SUCSESS, "Ok"));

                return runTmdsDbusPositiveMock.Object;
            }
          
            var runTmdsDbusNegativeMock = new Mock<RemoteRunTmdsDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runTmdsDbusNegativeMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunDbus);
            runTmdsDbusNegativeMock.Setup(m => m.Execute())
                .Returns(new Response(CommandType.eRunDbus, StatCodes.NO_CONTENT, "No content"));

            return runTmdsDbusNegativeMock.Object;

        }).As<RemoteRunTmdsDbusCmd>();
        
        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            
            string reqProcName = "foo-daemon.service";
            
            if (parameters.Positional<string>(1).Equals(reqProcName))
            {
                var stopDbusPositiveMock = new Mock<RemoteStopDbusCmd>(parameters.Positional<Guid>(0),
                    parameters.Positional<string>(1), parameters.Positional<string>(2));
                stopDbusPositiveMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
                stopDbusPositiveMock.Setup(m => m.Execute())
                    .Returns(new Response(CommandType.eStopDbus, StatCodes.SUCSESS, "Ok"));
            
                return stopDbusPositiveMock.Object;
            }
          
            var stopDbusNegMock = new Mock<RemoteStopDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopDbusNegMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
            stopDbusNegMock.Setup(m => m.Execute())
                .Returns(new Response(CommandType.eStopDbus, StatCodes.NO_CONTENT, "No content"));
            
            return stopDbusNegMock.Object;
        }).As<RemoteStopDbusCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            
            string reqProcName = "foo-daemon.service";

            if (parameters.Positional<string>(1).Equals(reqProcName))
            {
                var stopTmdsDbusPositiveMock = new Mock<RemoteStopTmdsDbusCmd>(parameters.Positional<Guid>(0),
                    parameters.Positional<string>(1), parameters.Positional<string>(2));
                stopTmdsDbusPositiveMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
                stopTmdsDbusPositiveMock.Setup(m => m.Execute())
                    .Returns(new Response(CommandType.eStopDbus, StatCodes.SUCSESS, "Ok"));
            
                return stopTmdsDbusPositiveMock.Object;
            }
         
            var stopTmdsDbusNegativeMock = new Mock<RemoteStopTmdsDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopTmdsDbusNegativeMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
            stopTmdsDbusNegativeMock.Setup(m => m.Execute())
                .Returns(new Response(CommandType.eStopDbus, StatCodes.NO_CONTENT, "No content"));
            
            return stopTmdsDbusNegativeMock.Object;
         
        }).As<RemoteStopTmdsDbusCmd>();
    }
}