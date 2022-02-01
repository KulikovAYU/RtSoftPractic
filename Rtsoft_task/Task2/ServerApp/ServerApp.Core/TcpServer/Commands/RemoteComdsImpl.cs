using Newtonsoft.Json.Linq;
using SysMonitor;
using System;
using System.Diagnostics;
using System.Text;

namespace ServerApp.Core.Commands
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
                p?.WaitForExit();
                Console.WriteLine($"Invoked {this}");
                return new Response(GetIdent(), 200, $"{this} has been executed");
            } catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{this} hasn't been executed");
            }
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

                return new Response(GetIdent(), 200, $"{this} has been executed");
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{this} hasn't been executed");
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
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = $"-c \"{runScript}\"", RedirectStandardOutput = true, UseShellExecute = false };
                Process proc = new Process() { StartInfo = startInfo, };
                Console.WriteLine($"Invoked {this}");
              
                if (proc.Start())
                {
                    proc.WaitForExit();
                    SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, name_);
                    return new Response(GetIdent(), 200, $"{this} has been executed");
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{this} hasn't been executed");
            }

            return new Response(GetIdent(), 204, $"{this} hasn't been executed");
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
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = $"-c \"{stopScript}\"", RedirectStandardOutput = true, UseShellExecute = false };
                Process proc = new Process() { StartInfo = startInfo, };
                Console.WriteLine($"Invoked command {this}");

                SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, args_);

                if (proc.Start())
                {
                    proc.WaitForExit();
                    return new Response(GetIdent(), 200, $"{this} has been executed");
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, $"{this} hasn't been executed");
            }

            return new Response(GetIdent(), 204, $"{this} hasn't been executed");
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
                case CommandType.eRunDbus: return new RemoteRunDbusCmd(cmd, args).Execute();
                case CommandType.eStopDbus: return new RemoteStopDbusCmd(cmd, args).Execute();
               
                default:
                    break;
            }

            return new Response(CommandType.eUndef, 204, "Undefined command type");
        }
    }
}