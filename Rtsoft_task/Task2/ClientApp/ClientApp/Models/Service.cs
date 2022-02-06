using System.Collections.ObjectModel;
using ClientApp.Base;
using ClientApp.Client;

namespace ClientApp.Models
{
    public class Service : AbstractItem
    {
        public override RemoteProcCommand ActivateCmd => new(CommandType.eRunDbus) { Name = Name, Args = Args };
        public override RemoteProcCommand DeactivateCmd => new(CommandType.eStopDbus) { Name = Name, Args = Args };
        
        public ObservableCollection<CpuTime> LoadingsCPUGraph { get; private set; } = new(); // CPU loading
    }
}
