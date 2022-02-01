using ClientApp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MQTTnet;
using ProtoBuf;
using ReactiveUI;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ClientView.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IEventBus
    {
        public ConnectionPref Pref { get; set; }

        public List<Axis> XAxes { get; set; }

        public ObservableCollection<ISeries> ServiceSeries { get; set; } = new ObservableCollection<ISeries>();

        public ObservableCollection<ISeries> CoreTempSeries { get; set; } = new ObservableCollection<ISeries>();

        public MainWindowViewModel()
        {
            client = new Client(this);
            Pref = new ConnectionPref()
            {
                HostNameOrAdress = "127.0.0.1",
                PortNumber = 11000,
                UserName = "Anton"
            };

            XAxes = new List<Axis>
            {
                new()
                {
                    // Use the Label property to indicate the format of the labels in the axis
                    // The Labeler takes the value of the label as parameter and must return it as string
                    Labeler = (value) => " ",

                    // The MinStep property lets you define the minimum separation (in chart values scale)
                    // between every axis separator, in this case we don't want decimals,
                    // so lets force it to be greater or equals than 1
                    MinStep = 1,

                    // labels rotations is in degrees (0 - 360)
                    LabelsRotation = 0,

                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray, 2)
                }
            };
        }

        public string StatusText
        {
            get => statusText_; set
            {
                statusText_ = value;
                this.RaisePropertyChanged("StatusText");
            }
        }

        public string RemoteProcessName { get; set; } = new string("vlc");
        public string RemoteProcessArgs { get; set; } = new string("");

        public string RemoteDbusServiceName { get; set; } = new string("foo-daemon.service");
        public string RemoteSudoPwd { get; set; } = new string("");

        public ReactiveCommand<Unit, Task> EstablishConnectCommand => ReactiveCommand.Create(async () =>
        {
            await client.EstablishConnectionAsync(Pref);

            this.RaisePropertyChanged("RunRemoteProcessCommand");
            this.RaisePropertyChanged("StopRemoteProcessCommand");
            this.RaisePropertyChanged("RunDbusRemoteProcessCommand");
            this.RaisePropertyChanged("StopDbusRemoteProcessCommand");
        });


        public ReactiveCommand<Unit, Unit> RunRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand runremoteProcCmd = new RemoteProcCommand(CommandType.eRunProc) { Name = RemoteProcessName, Args = RemoteProcessArgs };
            client.SendMessage(runremoteProcCmd.ToJSON());
        }, this.WhenAnyValue(x => x.client.IsConnected));


        public ReactiveCommand<Unit, Unit> StopRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand stopremoteProcCmd = new RemoteProcCommand(CommandType.eStopProc) { Name = RemoteProcessName, Args = RemoteProcessArgs };
            client.SendMessage(stopremoteProcCmd.ToJSON());
        }, this.WhenAnyValue(x => x.client.IsConnected));

        public ReactiveCommand<Unit, Unit> RunDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand rundbusCmd = new RemoteProcCommand(CommandType.eRunDbus) { Name = RemoteDbusServiceName, Args = RemoteSudoPwd };
            client.SendMessage(rundbusCmd.ToJSON());
        }, this.WhenAnyValue(x => x.client.IsConnected));

        public ReactiveCommand<Unit, Unit> StopDbusRemoteProcessCommand => ReactiveCommand.Create(() =>
        {
            RemoteProcCommand stopDbusCmd = new RemoteProcCommand(CommandType.eStopDbus) { Name = RemoteDbusServiceName, Args = RemoteSudoPwd };
            client.SendMessage(stopDbusCmd.ToJSON());
        }, this.WhenAnyValue(x => x.client.IsConnected));

        public void Print(string message)
        {
            StatusText += $"[INFO] {message}\n";
        }

        public void Error(string message)
        {
            StatusText += $"[ERROR] {message}\n";
        }

        public void OnMqqtEvent(MqttApplicationMessageReceivedEventArgs args)
        {
            using (MemoryStream stream = new MemoryStream(args.ApplicationMessage.Payload))
            {
                if (args.ApplicationMessage.Topic.Equals("RemoteSrvrData/CPU temp"))
                {
                    var cpuTemp = Serializer.Deserialize<CpuTemp>(stream);
                    if (cpuTemp != null)
                    {
                        var series = CoreTempSeries.FirstOrDefault();
                        if (series != null)
                        {
                            Print($"[MQTT] Topic=>{args.ApplicationMessage.Topic}; {cpuTemp}");
                            ObservableCollection<ObservableValue>? values = series.Values as ObservableCollection<ObservableValue>;
                            values?.Add(new ObservableValue(cpuTemp.Value));
                        }
                    }
                }
                else if (args.ApplicationMessage.Topic.Equals("RemoteSrvrData/CPU loading"))
                {
                    var cpuTime = Serializer.Deserialize<CpuTime>(stream);
                    if (cpuTime != null)
                    {
                        var series = ServiceSeries.FirstOrDefault(x => x.Name == cpuTime.ServiceName);
                        if (series != null)
                        {
                            Print($"[MQTT] Topic=>{args.ApplicationMessage.Topic}; {cpuTime}");
                            ObservableCollection<ObservableValue>? values = series.Values as ObservableCollection<ObservableValue>;
                            values?.Add(new ObservableValue(cpuTime.Value));
                        }
                    }
                }
            }
            // Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //Console.WriteLine($"+ QoS = {args.ApplicationMessage.QualityOfServiceLevel}");
            //Console.WriteLine($"+ Retain = {args.ApplicationMessage.Retain}");
            //Console.WriteLine();
        }

        public void OnResponse(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Response resp = new Response();
            if (resp.FromJSON(message))
            {
                if (resp.StatusCode == 200)
                {
                    if (CommandType.eEStablishConnect == resp.Type)
                    {
                        Print(resp.Body);

                        AddUniqueMeasurement(CoreTempSeries, "CPU Temperature");

                        if (CoreTempSeries.Last() is LineSeries<ObservableValue> newSer)
                        {
                            var red = new SKColor(229, 57, 53);
                            newSer.Stroke = new SolidColorPaint(red, 5);
                            newSer.GeometrySize = 5;
                            newSer.GeometryStroke = new SolidColorPaint(red, 2);
                            newSer.LineSmoothness = 1;
                        }
                    }
                    else if (CommandType.eRunDbus == resp.Type)
                    {
                        AddUniqueMeasurement(ServiceSeries, resp.Body);
                    }
                }
                else
                {
                    Error(resp.Body);
                }
            }
        }

        void AddUniqueMeasurement(ObservableCollection<ISeries> srcSeries, string seriesName)
        {
            if (srcSeries.FirstOrDefault(s => s.Name?.Equals(seriesName) == true) != null)
                return;
            
            var processSeries = new LineSeries<ObservableValue>
            {
                Values = new ObservableCollection<ObservableValue>(),
                Name = seriesName,
                GeometrySize = 5,
                LineSmoothness = 1 // mark
            };

            srcSeries.Add(processSeries);
        }

        public Client client { get; private set; }

        private string statusText_ = new string("");
    }
}
