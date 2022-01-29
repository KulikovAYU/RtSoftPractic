namespace ServerApp.Core.Commands
{
    public enum CommandType { eUndef = -1, eEStablishConnect, eRunProc, eStopProc, eRunDbus, eStopDbus }

    interface IRemoteCmd
    {
        CommandType GetIdent();

        Response Execute(string name, string args);
    }
}