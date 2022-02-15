using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Autofac;
using ServerApp.Core.Modules;
using SysMonitor;
using systemd1.DBus;
using Tmds.DBus;

namespace ServerApp.Core.Server.Commands
{
    public class RemoteRunProcCmd : AbstractRemoteCmd
    {
        public RemoteRunProcCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eRunProc;

        public override Response Execute()
        {
            try
            {
                using var p = Process.Start(new ProcessStartInfo
                {
                    FileName = Name, //file to execute
                    Arguments = Args,//arguments to use
                    UseShellExecute = false,//use process Creation semantics
                    RedirectStandardOutput = true, //redirect standart output to this proc object
                    CreateNoWindow = false,//if this is a terminal app, don't show it
                    WindowStyle = ProcessWindowStyle.Normal //if this is a terminal app, don't show it
                });
                
                Thread.Sleep(1000); // sleep for one second

                if (p?.Id > 0)
                {
                    Console.WriteLine($"Invoked {this}");
                    return new CommandResponse(Guid,GetIdent(), StatCodes.SUCSESS, $"{Name}");
                }
            } catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
            
            return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
        }
    }

    public class RemoteStopProcCmd : AbstractRemoteCmd
    {

        public RemoteStopProcCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eStopProc;

        public override Response Execute()
        {
            try
            {
                var workers = Process.GetProcessesByName(Name);
                if(workers.Length == 0)
                    return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"Remote host doesn't contains {this}");

                foreach (Process worker in workers)
                {
                    Console.WriteLine($"Invoked command {this}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return new CommandResponse(Guid, GetIdent(), StatCodes.SUCSESS, $"{Name}");
            }
            catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
        }
    }

    public class RemoteRunDbusCmd : AbstractRemoteCmd
    {
        public RemoteRunDbusCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eRunDbus;

        public override Response Execute()
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StartUnit string:"foo-daemon.service" string:"replace"
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"echo {Args} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StartUnit ").
                   Append($"string:\"{Name}\" ").
                   Append("string:\"replace\"");

                var runScript = sb.ToString();
                if (Utils.ExecuteScript(out _, runScript))
                {
                    Thread.Sleep(1000); // sleep for one second
                    if (Utils.GetProcIdByServiceName(out _,Name))
                    {
                        //TODO: вклячть зависимость от SysMonitorsPool через интерфейс
                        SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, Name);
                        return new CommandResponse(Guid, GetIdent(), StatCodes.SUCSESS, $"{Name}");
                    }
                }
            }
            catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
            
            return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
        }
    }

    public class RemoteRunTmdsDbusCmd : RemoteRunDbusCmd
    {
        public RemoteRunTmdsDbusCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override Response Execute()
        {
            try
            {
                var systemConnection = Connection.System; //sys bus

                var systemd1Path = new ObjectPath("/org/freedesktop/systemd1");
                var networkManager = systemConnection.CreateProxy<IManager>("org.freedesktop.systemd1",
                    systemd1Path);

                var result = networkManager.StartUnitAsync(Name, "replace");
                result.Wait();
                
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, Name);
                    return new CommandResponse(Guid, GetIdent(), StatCodes.SUCSESS, $"{Name}");
                }
            }
            catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
            
            return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
        }
    }

    public class RemoteStopDbusCmd : AbstractRemoteCmd
    {
        public RemoteStopDbusCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eStopDbus;

        public override Response Execute()
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StopUnit string:"foo-daemon.service" string:"fail"
            try
            {
                 if (!Utils.GetProcIdByServiceName(out _,Name))
                     return new CommandResponse(Guid, GetIdent(),  StatCodes.NO_CONTENT, $"{Name}");
                
                 StringBuilder sb = new StringBuilder();
                 sb.Append($"echo {Args} | ").
                     Append("sudo -S dbus-send ").
                     Append("--print-reply ").
                     Append("--system ").
                     Append("--type=method_call ").
                     Append("--dest=org.freedesktop.systemd1 ").
                     Append("/org/freedesktop/systemd1 ").
                     Append("org.freedesktop.systemd1.Manager.StopUnit ").
                     Append($"string:\"{Name}\" ").
                     Append("string:\"fail\"");

                 var stopScript = sb.ToString();

                 if (Utils.ExecuteScript(out _, stopScript))
                 {
                     Thread.Sleep(1000); // sleep for one second
                     SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, Name);
                     Console.WriteLine($"Invoked command {this}");
                     return new CommandResponse(Guid, GetIdent(), StatCodes.SUCSESS, $"{Name}");
                 }
            }
            catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
            
            return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
        }
    }


    public class RemoteStopTmdsDbusCmd : RemoteStopDbusCmd
    {
        public RemoteStopTmdsDbusCmd(Guid guid, string name, string args) : base(guid, name, args)
        {
        }

        public override Response Execute()
        {
            try
            {
                var systemConnection = Connection.System; //sys bus

                var systemd1Path = new ObjectPath("/org/freedesktop/systemd1");
                var networkManager = systemConnection.CreateProxy<IManager>("org.freedesktop.systemd1",
                    systemd1Path);

                var result = networkManager.StopUnitAsync(Name, "fail");
                result.Wait();
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, Name);
                    return new CommandResponse(Guid, GetIdent(), StatCodes.SUCSESS, $"{Name}");
                }
            }
            catch (Exception)
            {
                return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
            }
            
            return new CommandResponse(Guid, GetIdent(), StatCodes.NO_CONTENT, $"{Name}");
        }
    }

    static class CommandExecutor
    {
        public static Response FromJson(string cmdJson)
        {
            try
            {
                var clientCmd = ClientCommand.FromJson(cmdJson);

                var guid = new PositionalParameter(0, clientCmd.Guid);
                var name = new PositionalParameter(1, clientCmd.Name);
                var cmd = new PositionalParameter(2, clientCmd.Args);
                
                switch (clientCmd.Type)
                {
                    case CommandType.eRunProc:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteRunProcCmd>(guid, name, cmd).Execute();
                    case CommandType.eStopProc:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteStopProcCmd>(guid, name, cmd).Execute();
                    case CommandType.eRunDbus:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteRunTmdsDbusCmd>(guid, name, cmd).Execute();
                    case CommandType.eStopDbus:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteStopTmdsDbusCmd>(guid, name, cmd).Execute();
                }
            }
            catch (Exception e)
            {
               return new CommandResponse(Guid.Empty, CommandType.eUndef,  StatCodes.BAD_REQUEST, e.Message);
            }

            return new CommandResponse(Guid.Empty, CommandType.eUndef, StatCodes.NO_CONTENT, "Undefined command type");
        }
    }
}