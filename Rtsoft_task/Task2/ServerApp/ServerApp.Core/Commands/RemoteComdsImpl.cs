using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;


namespace ServerApp.Core.Commands
{
    public class RemoteRunProcCmd : IRemoteCmd
    {
        CommandType IRemoteCmd.GetIdent() => CommandType.eRunProc;

        public bool Execute(string name, string args = null)
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
                    return true;
                }

            } catch (Exception)
            {
            
            
            }
            

           
            return false;
        }

       
    }

    public class RemoteStopProcCmd : IRemoteCmd
    {
        CommandType IRemoteCmd.GetIdent() => CommandType.eStopProc;

        public bool Execute(string name, string args = null)
        {
            try
            {
                var currProc =  Process.GetCurrentProcess();
                var workers = Process.GetProcessesByName(name);
                foreach (Process worker in workers)
                {
                    Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                }

                return true;
            }
            catch (Exception)
            {


            }

            return false;
        }
    }

    public class RemoteRunDbusCmd : IRemoteCmd
    {
        CommandType IRemoteCmd.GetIdent() => CommandType.eRunDbus;

        public bool Execute(string name, string args = null)
        {
            //Connection.Session.CreateProxy<>

            Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
            return true;
        }
    }

    public class RemoteStopDbusCmd : IRemoteCmd
    {
        CommandType IRemoteCmd.GetIdent() => CommandType.eStopDbus;

        public bool Execute(string name, string args = null)
        {
            Console.WriteLine($"Invoked command {GetType()} ; name = {name}; ags = {args}");
            return true;
        }
    }


    public class CommandExecutor
    {
        public static bool FromJSON(string cmdJSON)
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

            return false;
        }
    }

    
}
