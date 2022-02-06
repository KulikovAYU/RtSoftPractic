using System;

namespace ServerApp.Core.TcpServer.Commands
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    public abstract class AbstractRemoteCmd
    {
        protected readonly Guid guid_;
        protected readonly string name_;
        protected readonly string args_;

        protected AbstractRemoteCmd(Guid guid,string name, string args)
        {
            guid_ = guid;
            name_ = name;
            args_ = args;
        }

        public abstract CommandType GetIdent();

        public abstract Response Execute();

        public override string ToString()
        {
            return $"Command {GetType()} ; name = {name_}; ags = {args_}";
        }
    }
}