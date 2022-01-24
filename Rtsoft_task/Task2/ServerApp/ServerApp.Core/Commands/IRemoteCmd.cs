namespace ServerApp.Core.Commands
{
    public enum CommandType { eUndef = -1, eRunProc, eStopProc, eRunDbus, eStopDbus }

    interface IRemoteCmd
    {
        CommandType GetIdent();

        bool Execute(string name, string args = null);
    }
}