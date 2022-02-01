namespace ServerApp.Core.TcpServer.Commands
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    public abstract class AbstractRemoteCmd
    {
        protected readonly string name_;
        protected readonly string args_;

        protected AbstractRemoteCmd(string name, string args)
        {
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