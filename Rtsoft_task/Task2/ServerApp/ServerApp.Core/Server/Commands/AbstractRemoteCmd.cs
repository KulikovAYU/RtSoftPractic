using System;

namespace ServerApp.Core.Server.Commands
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    public abstract class AbstractRemoteCmd
    {
        public Guid Guid { get; }
        public string Name { get; }
        public string Args { get; }

        protected AbstractRemoteCmd(Guid guid,string name, string args)
        {
            Guid = guid;
            Name = name;
            Args = args;
        }

        public abstract CommandType GetIdent();

        public abstract Response Execute();

        public override string ToString() => $"Command {GetType()} ; name = {Name}; ags = {Args}";
    }
}