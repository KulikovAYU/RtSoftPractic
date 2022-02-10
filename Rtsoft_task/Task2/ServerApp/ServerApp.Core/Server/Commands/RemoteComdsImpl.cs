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
                    FileName = _name, //file to execute
                    Arguments = _args,//arguments to use
                    UseShellExecute = false,//use process Creation semantics
                    RedirectStandardOutput = true, //redirect standart output to this proc object
                    CreateNoWindow = false,//if this is a terminal app, don't show it
                    WindowStyle = ProcessWindowStyle.Normal //if this is a terminal app, don't show it
                });
                
                Thread.Sleep(1000); // sleep for one second

                if (p?.Id > 0)
                {
                    Console.WriteLine($"Invoked {this}");
                    return new CommandResponse(_guid,GetIdent(), 200, $"{_name}");
                }
            } catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
            }
            
            return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
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
                var workers = Process.GetProcessesByName(_name);
                if(workers.Length == 0)
                    return new CommandResponse(_guid, GetIdent(), 204, $"Remote host doesn't contains {this}");

                foreach (Process worker in workers)
                {
                    Console.WriteLine($"Invoked command {this}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return new CommandResponse(_guid, GetIdent(), 200, $"{_name}");
            }
            catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
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
                sb.Append($"echo {_args} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StartUnit ").
                   Append($"string:\"{_name}\" ").
                   Append("string:\"replace\"");

                var runScript = sb.ToString();
                if (Utils.ExecuteScript(out _, runScript))
                {
                    Thread.Sleep(1000); // sleep for one second
                    if (Utils.GetProcIdByServiceName(out _,_name))
                    {
                        //TODO: вклячть зависимость от SysMonitorsPool через интерфейс
                        SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, _name);
                        return new CommandResponse(_guid, GetIdent(), 200, $"{_name}");
                    }
                }
            }
            catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
            }
            
            return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
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

                var result = networkManager.StartUnitAsync(_name, "replace");
                result.Wait();
                
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, _name);
                    return new CommandResponse(_guid, GetIdent(), 200, $"{_name}");
                }
            }
            catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
            }
            
            return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
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
                 if (!Utils.GetProcIdByServiceName(out _,_name))
                     return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
                
                 StringBuilder sb = new StringBuilder();
                 sb.Append($"echo {_args} | ").
                     Append("sudo -S dbus-send ").
                     Append("--print-reply ").
                     Append("--system ").
                     Append("--type=method_call ").
                     Append("--dest=org.freedesktop.systemd1 ").
                     Append("/org/freedesktop/systemd1 ").
                     Append("org.freedesktop.systemd1.Manager.StopUnit ").
                     Append($"string:\"{_name}\" ").
                     Append("string:\"fail\"");

                 var stopScript = sb.ToString();

                 if (Utils.ExecuteScript(out _, stopScript))
                 {
                     Thread.Sleep(1000); // sleep for one second
                     SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, _name);
                     Console.WriteLine($"Invoked command {this}");
                     return new CommandResponse(_guid, GetIdent(), 200, $"{_name}");
                 }
            }
            catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
            }
            
            return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
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

                var result = networkManager.StopUnitAsync(_name, "fail");
                result.Wait();
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, _name);
                    return new CommandResponse(_guid, GetIdent(), 200, $"{_name}");
                }
            }
            catch (Exception)
            {
                return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
            }
            
            return new CommandResponse(_guid, GetIdent(), 204, $"{_name}");
        }
    }

    static class CommandExecutor
    {
        public static Response FromJson(string cmdJson)
        {
            try
            {
                var clientCmd = ClientCommand.FromJson(cmdJson);

                switch (clientCmd.Type)
                {
                    case CommandType.eRunProc:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteRunProcCmd>( new PositionalParameter(0, clientCmd.Guid), new PositionalParameter(1, clientCmd.Name), new PositionalParameter(2, clientCmd.Args)).Execute();
                    case CommandType.eStopProc:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteStopProcCmd>( new PositionalParameter(0, clientCmd.Guid), new PositionalParameter(1, clientCmd.Name), new PositionalParameter(2, clientCmd.Args)).Execute();
                    case CommandType.eRunDbus:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteRunTmdsDbusCmd>( new PositionalParameter(0, clientCmd.Guid), new PositionalParameter(1, clientCmd.Name), new PositionalParameter(2, clientCmd.Args)).Execute();
                    case CommandType.eStopDbus:
                        return ContainerOfModulesBase.Ioc.Resolve<RemoteStopTmdsDbusCmd>( new PositionalParameter(0, clientCmd.Guid), new PositionalParameter(1, clientCmd.Name), new PositionalParameter(2, clientCmd.Args)).Execute();
                }
            }
            catch (Exception e)
            {
               return new CommandResponse(Guid.Empty, CommandType.eUndef, 400, e.Message);
            }

            return new CommandResponse(Guid.Empty, CommandType.eUndef, 204, "Undefined command type");
        }
    }
}