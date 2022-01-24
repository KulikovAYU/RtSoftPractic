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

        public string RemoteProcessName { get; set; } = new string("mspaint");
        public string RemoteProcessArgs { get; set; } = new string("");


        public string RemoteDbusCommand { get; set; } = new string("");

        public ReactiveCommand<Unit, Unit> EstablishConnectCommand => ReactiveCommand.Create(() => {

          //  this.RaisePropertyChanged(nameof(StatusText));

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
            //TODO: Сервер принимает json команду на запуск/останов процессе с заданным именем

            RemoteProcCommand runCmd = new RemoteProcCommand(CommandType.eStopProc) { Name = RemoteProcessName, Args = RemoteProcessArgs };
            var jsonCommand = JsonConvert.SerializeObject(runCmd);
            client.SendMessage(jsonCommand);
        });

        public ReactiveCommand<Unit, Unit> RunDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            int n = 1;


            //  this.RaisePropertyChanged(nameof(StatusText));

            //client.EstablishConnection(Pref);
        });

        public ReactiveCommand<Unit, Unit> StopDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            int n = 1;
            //  this.RaisePropertyChanged(nameof(StatusText));

            //client.EstablishConnection(Pref);
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
