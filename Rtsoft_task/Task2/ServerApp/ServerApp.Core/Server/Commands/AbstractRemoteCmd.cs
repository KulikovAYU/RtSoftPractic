using System;

namespace ServerApp.Core.Server.Commands
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    public abstract class AbstractRemoteCmd
    {
        protected readonly Guid _guid;
        protected readonly string _name;
        protected readonly string _args;

        protected AbstractRemoteCmd(Guid guid,string name, string args)
        {
            _guid = guid;
            _name = name;
            _args = args;
        }

        public abstract CommandType GetIdent();

        public abstract Response Execute();

        public override string ToString() => $"Command {GetType()} ; name = {_name}; ags = {_args}";
    }
}