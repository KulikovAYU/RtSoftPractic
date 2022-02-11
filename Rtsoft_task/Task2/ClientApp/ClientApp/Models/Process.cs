using ClientApp.Client;

namespace ClientApp.Models
{
    public class Process : AbstractItem
    {
        public override RemoteProcCommand ActivateCmd => new(CommandType.eRunProc) { Name = Name, Args = Args, Guid = Guid};
        public override RemoteProcCommand DeactivateCmd => new(CommandType.eStopProc) { Name = Name, Args = Args, Guid = Guid };
    }
}
