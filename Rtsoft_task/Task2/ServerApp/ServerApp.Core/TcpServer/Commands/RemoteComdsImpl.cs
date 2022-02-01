using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using SysMonitor;
using systemd1.DBus;
using Tmds.DBus;

namespace ServerApp.Core.TcpServer.Commands
{
    public class RemoteRunProcCmd : AbstractRemoteCmd
    {
        public RemoteRunProcCmd(string name, string args) : base(name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eRunProc;

        public override Response Execute()
        {
            try
            {
                using var p = Process.Start(new ProcessStartInfo
                {
                    FileName = name_, //file to execute
                    Arguments = args_,//arguments to use
                    UseShellExecute = false,//use process Creation semantics
                    RedirectStandardOutput = true, //redirect standart output to this proc object
                    CreateNoWindow = false,//if this is a terminal app, don't show it
                    WindowStyle = ProcessWindowStyle.Normal //if this is a terminal app, don't show it
                });
                
                Thread.Sleep(1000); // sleep for one second

                if (p?.Id > 0)
                {
                    Console.WriteLine($"Invoked {this}");
                    return new Response(GetIdent(), 200, $"{name_}");
                }
            } catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
            
            return new Response(GetIdent(), 204, $"{name_}");
        }
    }

    public class RemoteStopProcCmd : AbstractRemoteCmd
    {

        public RemoteStopProcCmd(string name, string args) : base(name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eStopProc;

        public override Response Execute()
        {
            try
            {
                var workers = Process.GetProcessesByName(name_);
                if(workers.Length == 0)
                    return new Response(GetIdent(), 204, $"Remote host doesn't contains {this}");

                foreach (Process worker in workers)
                {
                    Console.WriteLine($"Invoked command {this}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return new Response(GetIdent(), 200, $"{name_}");
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
        }
    }

    public class RemoteRunDbusCmd : AbstractRemoteCmd
    {
        public RemoteRunDbusCmd(string name, string args) : base(name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eRunDbus;

        public override Response Execute()
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StartUnit string:"foo-daemon.service" string:"replace"
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"echo {args_} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StartUnit ").
                   Append($"string:\"{name_}\" ").
                   Append("string:\"replace\"");

                var runScript = sb.ToString();
                if (Utils.ExecuteScript(out _, runScript))
                {
                    Thread.Sleep(1000); // sleep for one second
                    if (Utils.GetProcIdByServiceName(out _,name_))
                    {
                        SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, name_);
                        return new Response(GetIdent(), 200, $"{name_}");
                    }
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
            
            return new Response(GetIdent(), 204, $"{name_}");
        }
    }


    public class RemoteRunTmdsDbusCmd : RemoteRunDbusCmd
    {
        public RemoteRunTmdsDbusCmd(string name, string args) : base(name, args)
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

                var result = networkManager.StartUnitAsync(name_, "replace");
                result.Wait();
                
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, name_);
                    return new Response(GetIdent(), 200, $"{name_}");
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
            
            return new Response(GetIdent(), 204, $"{name_}");
        }
        
    }

    public class RemoteStopDbusCmd : AbstractRemoteCmd
    {
        public RemoteStopDbusCmd(string name, string args) : base(name, args)
        {
        }

        public override CommandType GetIdent() => CommandType.eStopDbus;

        public override Response Execute()
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StopUnit string:"foo-daemon.service" string:"fail"
            try
            {
                 if (!Utils.GetProcIdByServiceName(out _,name_))
                     return new Response(GetIdent(), 204, $"{name_}");
                
                StringBuilder sb = new StringBuilder();
                sb.Append($"echo {args_} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StopUnit ").
                   Append($"string:\"{name_}\" ").
                   Append("string:\"fail\"");

                var stopScript = sb.ToString();

                if (Utils.ExecuteScript(out _, stopScript))
                {
                    Thread.Sleep(1000); // sleep for one second
                    SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, name_);
                    Console.WriteLine($"Invoked command {this}");
                    return new Response(GetIdent(), 200, $"{name_}");
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
            
            return new Response(GetIdent(), 204, $"{name_}");
        }
    }


    public class RemoteStopTmdsDbusCmd : RemoteRunDbusCmd
    {
        public RemoteStopTmdsDbusCmd(string name, string args) : base(name, args)
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

                var result = networkManager.StopUnitAsync(name_, "fail");
                result.Wait();
                if (result.IsCompletedSuccessfully)
                {
                    SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, name_);
                    return new Response(GetIdent(), 200, $"{name_}");
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{name_}");
            }
            
            return new Response(GetIdent(), 204, $"{name_}");
        }
    }

    public static class CommandExecutor
    {
        public static Response FromJson(string cmdJson)
        {
            JObject jObject = JObject.Parse(cmdJson);
            CommandType cmdType = (CommandType)int.Parse(jObject["Type"]?.ToString() ?? string.Empty);
            string cmd = jObject["Name"]?.ToString();
            string args = jObject["Args"]?.ToString();

            switch (cmdType)
            {
                case CommandType.eRunProc: return new RemoteRunProcCmd(cmd, args).Execute();
                case CommandType.eStopProc: return new RemoteStopProcCmd(cmd, args).Execute();
                case CommandType.eRunDbus: return new RemoteRunTmdsDbusCmd(cmd, args).Execute();
                case CommandType.eStopDbus: return new RemoteStopTmdsDbusCmd(cmd, args).Execute();
            }

            return new Response(CommandType.eUndef, 204, "Undefined command type");
        }
    }
}