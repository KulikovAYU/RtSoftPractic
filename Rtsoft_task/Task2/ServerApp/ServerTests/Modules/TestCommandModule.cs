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
            var parameters = p.ToList();
            var runProcMock = new Mock<RemoteRunProcCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runProcMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunProc);
            runProcMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eRunDbus, 200, "Ok"));
            return runProcMock.Object;
        }).As<RemoteRunProcCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            var stopProcMock = new Mock<RemoteStopProcCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopProcMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopProc);
            stopProcMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eRunDbus, 200, "Ok"));
            return stopProcMock.Object;
        }).As<RemoteStopProcCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            var runDbusMock = new Mock<RemoteRunDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runDbusMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunDbus);
            runDbusMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eRunDbus, 200, "Ok"));
            return runDbusMock.Object;
        }).As<RemoteRunDbusCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            var runDbusMock = new Mock<RemoteStopDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            runDbusMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
            runDbusMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eStopDbus, 200, "Ok"));
            return runDbusMock.Object;
        }).As<RemoteStopDbusCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            var stopDbusMock = new Mock<RemoteRunTmdsDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopDbusMock.Setup(m => m.GetIdent()).Returns(CommandType.eRunDbus);
            stopDbusMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eRunDbus, 200, "Ok"));
            return stopDbusMock.Object;
        }).As<RemoteRunTmdsDbusCmd>();

        builder.Register((c, p) =>
        {
            var parameters = p.ToList();
            var stopDbusMock = new Mock<RemoteStopTmdsDbusCmd>(parameters.Positional<Guid>(0),
                parameters.Positional<string>(1), parameters.Positional<string>(2));
            stopDbusMock.Setup(m => m.GetIdent()).Returns(CommandType.eStopDbus);
            stopDbusMock.Setup(m => m.Execute()).Returns(new Response(CommandType.eStopDbus, 200, "Ok"));
            return stopDbusMock.Object;
        }).As<RemoteStopTmdsDbusCmd>();
    }
}