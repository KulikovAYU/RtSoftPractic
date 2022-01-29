using Newtonsoft.Json.Linq;
using SysMonitor;
using System;
using System.Diagnostics;
using System.Text;

namespace ServerApp.Core.Commands
{
    public class RemoteRunProcCmd : IRemoteCmd
    {
        public CommandType GetIdent() => CommandType.eRunProc;

        public Response Execute(string name, string args)
        {
            try
            {
                using (var p = Process.Start(new ProcessStartInfo
                {
                    FileName = name, //file to execute
                    Arguments = args,//arguments to use
                    UseShellExecute = false,//use process Creation semantics
                    RedirectStandardOutput = true, //redirect standart output to this proc object
                    CreateNoWindow = false,//if this is a terminal app, don't show it
                    WindowStyle = ProcessWindowStyle.Normal //if this is a terminal app, don't show it
                }))
                {
                    Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
                    return new Response(GetIdent(), 200, string.Empty);
                }

            } catch (Exception)
            {
                return new Response(GetIdent(), 204, string.Empty);
            }
        }
    }

    public class RemoteStopProcCmd : IRemoteCmd
    {
        public CommandType GetIdent() => CommandType.eStopProc;

        public Response Execute(string name, string args)
        {
            try
            {
                var workers = Process.GetProcessesByName(name);
                foreach (Process worker in workers)
                {
                    Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return new Response(GetIdent(), 200, string.Empty); ;
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, string.Empty);
            }
        }
    }


    public class RemoteRunDbusCmd : IRemoteCmd
    {
        public CommandType GetIdent() => CommandType.eRunDbus;

        public Response Execute(string name, string args)
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StartUnit string:"foo-daemon.service" string:"replace"
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"echo {args} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StartUnit ").
                   Append($"string:\"{name}\" ").
                   Append("string:\"replace\"");

                var runScript = sb.ToString();
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = $"-c \"{runScript}\"", RedirectStandardOutput = true, UseShellExecute = false };
                Process proc = new Process() { StartInfo = startInfo, };
                Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
              
                if (proc.Start())
                {
                    SysMonitorsPool.CreateDevice(DevidceType.eCPUMonitor, name);
                    return new Response(GetIdent(), 200, name);
                }

            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, string.Empty);
            }

            return new Response(GetIdent(), 204, string.Empty);
        }
    }

    public class RemoteStopDbusCmd : IRemoteCmd
    {
        public CommandType GetIdent() => CommandType.eStopDbus;

        public Response Execute(string name, string args)
        {
            //echo 27051989 | sudo -S dbus-send --print-reply --system --type=method_call --dest=org.freedesktop.systemd1 /org/freedesktop/systemd1 org.freedesktop.systemd1.Manager.StopUnit string:"foo-daemon.service" string:"fail"
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"echo {args} | ").
                   Append("sudo -S dbus-send ").
                   Append("--print-reply ").
                   Append("--system ").
                   Append("--type=method_call ").
                   Append("--dest=org.freedesktop.systemd1 ").
                   Append("/org/freedesktop/systemd1 ").
                   Append("org.freedesktop.systemd1.Manager.StopUnit ").
                   Append($"string:\"{name}\" ").
                   Append("string:\"fail\"");

                var stopScript = sb.ToString();
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = $"-c \"{stopScript}\"", RedirectStandardOutput = true, UseShellExecute = false };
                Process proc = new Process() { StartInfo = startInfo, };
                Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");

                SysMonitorsPool.RemoveDevice(DevidceType.eCPUMonitor, name);

                if (proc.Start())
                {
                    return new Response(GetIdent(), 200, name);
                }
            }
            catch (Exception)
            {
                return new Response(GetIdent(), 204, string.Empty);
            }

            return new Response(GetIdent(), 204, string.Empty);
        }
    }

    public class CommandExecutor
    {
        public static Response FromJSON(string cmdJSON)
        {
            JObject jObject = JObject.Parse(cmdJSON);
            CommandType cmdType = (CommandType)int.Parse(jObject["Type"].ToString());
            string cmd = jObject["Name"].ToString();
            string args = jObject["Args"].ToString();

            switch (cmdType)
            {
                case CommandType.eRunProc: return new RemoteRunProcCmd().Execute(cmd, args);
                case CommandType.eStopProc: return new RemoteStopProcCmd().Execute(cmd, args);
                case CommandType.eRunDbus: return new RemoteRunDbusCmd().Execute(cmd, args);
                case CommandType.eStopDbus: return new RemoteStopDbusCmd().Execute(cmd, args);
               
                default:
                    break;
            }

            return new Response(CommandType.eUndef, 204, string.Empty);
        }
    }
}