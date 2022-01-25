using ClientApp;
using Newtonsoft.Json;
using ReactiveUI;
using System.Reactive;

namespace ClientView.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IEventBus
    {
        public ConnectionPref Pref { get; set; }
        public MainWindowViewModel()
        {
            client = new Client1(this);
            Pref = new ConnectionPref()
            {
                HostNameOrAdress = "localhost",
                PortNumber = 11000,
                UserName = "Anton"
            };
        }

        public string StatusText { get=> statusText_; set { statusText_ = value; 
                this.RaisePropertyChanged(); } }

        public string RemoteProcessName { get; set; } = new string("vlc");
        public string RemoteProcessArgs { get; set; } = new string("");


        public string RemoteDbusServiceName { get; set; } = new string("foo-daemon.service");
        public string RemoteSudoPwd { get; set; } = new string("");

        public ReactiveCommand<Unit, Unit> EstablishConnectCommand => ReactiveCommand.Create(() => {
            client.EstablishConnection(Pref);
        });

        public ReactiveCommand<Unit, Unit> RunRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand runCmd = new RemoteProcCommand(CommandType.eRunProc) { Name = RemoteProcessName, Args = RemoteProcessArgs };
            var jsonCommand = JsonConvert.SerializeObject(runCmd);
            client.SendMessage(jsonCommand);
        });

        public ReactiveCommand<Unit, Unit> StopRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand stopCmd = new RemoteProcCommand(CommandType.eStopProc) { Name = RemoteProcessName, Args = RemoteProcessArgs };
            var jsonCommand = JsonConvert.SerializeObject(stopCmd);
            client.SendMessage(jsonCommand);
        });

        public ReactiveCommand<Unit, Unit> RunDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand runCmd = new RemoteProcCommand(CommandType.eRunDbus) { Name = RemoteDbusServiceName, Args = RemoteSudoPwd };
            var jsonCommand = JsonConvert.SerializeObject(runCmd);
            client.SendMessage(jsonCommand);
        });

        public ReactiveCommand<Unit, Unit> StopDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand runCmd = new RemoteProcCommand(CommandType.eStopDbus) { Name = RemoteDbusServiceName, Args = RemoteSudoPwd };
            var jsonCommand = JsonConvert.SerializeObject(runCmd);
            client.SendMessage(jsonCommand);
        });

        public void Print(string message)
        {
            StatusText += $"[INFO] {message}\n";
        }

        public void Error(string message)
        {
            StatusText += $"[ERROR] {message}\n";
        }

        private Client1 client;

        private string statusText_ = new string("");

        
    }
}
