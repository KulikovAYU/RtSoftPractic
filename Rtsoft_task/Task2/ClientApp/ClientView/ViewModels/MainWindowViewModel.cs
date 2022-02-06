using System;
using ClientApp.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MQTTnet;
using ProtoBuf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ClientApp.Base;
using ClientApp.Client;
using DynamicData.Binding;


namespace ClientView.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IEventBus
    {
        #region Properties
        public ConnectionPref Pref { get; set; }
        public List<Axis> XAxes { get; set; }

        public ObservableCollection<ISeries> ServiceSeries { get; set; } = new();
        public ObservableCollection<ISeries> CoreTempSeries { get; set; } = new();
        public ObservableCollection<AbstractItem> ActiveServices { get; set; } = new();

        [Reactive] public string RemoteProcessName { get; set; } = new("vlc");
        [Reactive] public string RemoteProcessArgs { get; set; } = new("");

        [Reactive] public string RemoteDbusServiceName { get; set; } = new("foo-daemon.service");
        [Reactive] public string RemoteSudoPwd { get; set; } = new("");

        [Reactive] public string StatusText { get; private set;} = new("");

        [Reactive] public Client Client { get; private set; }
        
        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            Client = new Client(this);

            Client.WhenAnyPropertyChanged()
                .Subscribe(_ => {
                    this.RaisePropertyChanged(nameof(RunStopCommand));
                    this.RaisePropertyChanged(nameof(EstablishConnectCommand));
                    this.RaisePropertyChanged(nameof(BreakConnectCommand));
                });
            
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
                    Labeler = (value) => "",

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

        #endregion

        #region Connect/Disconnect commands

        public ReactiveCommand<Unit, Task> EstablishConnectCommand =>ReactiveCommand.Create(async () =>
        {
            await Client.EstablishConnectionAsync(Pref);
          
        },  this.WhenAnyValue(vm=> vm.Client.IsConnected, (isConnected)=>!isConnected));
        
        public ReactiveCommand<Unit, Unit> BreakConnectCommand => ReactiveCommand.Create(() =>
        {
            Client.DisconnectCommand();
        }, Client.WhenAnyValue(client=> client.IsConnected));
        
        #endregion
     
        #region Invoke commands (Run/Stop Process, Run/Stop Dbus)
        
        public ReactiveCommand<AbstractItem, Unit> RunStopCommand => ReactiveCommand.Create<AbstractItem>((itm) =>
        {
            if (itm.Status == State.Stopped)
            {
                Client.SendMessage(itm.ActivateCmd.ToJSON().ToString());
            }
            else if (itm.Status == State.Started)
            {
                Client.SendMessage(itm.DeactivateCmd.ToJSON());
            }
        }, Client.WhenAnyValue(client => client.IsConnected));
        
        #endregion

        #region Command to run/stop remote process

        // public ReactiveCommand<AbstractItem, Unit> RunStopRemoteProcessCommand => ReactiveCommand.Create<AbstractItem>((itm) =>
        // {
        //     if (itm.Status == State.Stopped)
        //     {
        //         Client.SendMessage(itm.ActivateCmd.ToJSON().ToString());
        //     }
        //     else if (itm.Status == State.Started)
        //     {
        //         Client.SendMessage(itm.DeactivateCmd.ToJSON());
        //     }
        // }, Client.WhenAnyValue(clnt => clnt.IsConnected));

        #endregion

        #region Command to runn/stop remote d bus service
        // public ReactiveCommand<AbstractItem, Unit> RunStopDbusRemoteProcessCommand => ReactiveCommand.Create<AbstractItem>((itm) =>
        // {
        //     if (itm.Status == State.Stopped)
        //     {
        //         Client.SendMessage(itm.ActivateCmd.ToJSON());
        //     }
        //     else if (itm.Status == State.Started)
        //     {
        //         Client.SendMessage(itm.DeactivateCmd.ToJSON());
        //     }
        // }, Client.WhenAnyValue(clnt => clnt.IsConnected));

        #endregion


        #region Command which creates executors and remove theirs
        
        public ReactiveCommand<string, Unit> NewExecutorCommand => ReactiveCommand.Create<string>((cmdName) =>
        {
            var newCmd = ServiceFactory.Create(cmdName);
            if (newCmd != null)
                ActiveServices.Add(newCmd);
        });
        
        public ReactiveCommand<AbstractItem, Unit> RemoteExecutorCommand => ReactiveCommand.Create<AbstractItem>((itm) =>
        {
            //TODO: may be stopped
            ActiveServices.Remove(itm);
        });
        
        #endregion
      
        public void Print(string message)
        {
            StatusText += $"[INFO] {message}\n";
        }

        public void Error(string message)
        {
            StatusText += $"[ERROR] {message}\n";
        }

        public async Task OnMqqtEvent(MqttApplicationMessageReceivedEventArgs args)
        {
            await Task.Run(() =>
            {
                using var stream = new MemoryStream(args.ApplicationMessage.Payload);
                if (args.ApplicationMessage.Topic.Equals("RemoteSrvrData/CPU temp"))
                {
                    var cpuTemp = Serializer.Deserialize<CpuTemp>(stream);
                    if (cpuTemp != null)
                    {
                        var series = CoreTempSeries.FirstOrDefault();

                        Print($"[MQTT] Topic=>{args.ApplicationMessage.Topic}; {cpuTemp}");
                        ObservableCollection<ObservableValue>? values = series?.Values as ObservableCollection<ObservableValue>;
                        values?.Add(new ObservableValue(cpuTemp.Value));
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
            });
            

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
                    Print($"Sucsess execution {resp.Body}");

                    switch (resp.Type)
                    {
                        case CommandType.eEStablishConnect:
                        {
                            if (AddUniqueMeasurement(CoreTempSeries, "CPU Temperature"))
                            {
                                if (CoreTempSeries.Last() is LineSeries<ObservableValue> newSer)
                                {
                                    var red = new SKColor(229, 57, 53);
                                    newSer.Stroke = new SolidColorPaint(red, 5);
                                    newSer.GeometrySize = 5;
                                    newSer.GeometryStroke = new SolidColorPaint(red, 2);
                                    newSer.LineSmoothness = 1;
                                }
                            }

                            break;
                        }
                        case CommandType.eRunDbus:
                        {
                            GetServiceByGuid(resp.Guid)?.Start();
                            AddUniqueMeasurement(ServiceSeries, resp.Body);
                            break;
                        }
                        case CommandType.eRunProc:
                        {
                            GetServiceByGuid(resp.Guid)?.Start();
                            break;
                        }
                        case CommandType.eStopDbus:
                        {
                            GetServiceByGuid(resp.Guid)?.Stop();
                            break;
                        }
                        case CommandType.eStopProc:
                        {
                            GetServiceByGuid(resp.Guid)?.Stop();
                            break;
                        }
                    }
                }
                else
                {
                    Error($"Failed execution {resp.Body}");
                }
            }
        }

        private bool AddUniqueMeasurement(ObservableCollection<ISeries> srcSeries, string seriesName)
        {
            if (srcSeries.FirstOrDefault(s => s.Name?.Equals(seriesName) == true) != null)
                return false;
            
            var processSeries = new LineSeries<ObservableValue>
            {
                Values = new ObservableCollection<ObservableValue>(),
                Name = seriesName,
                GeometrySize = 5,
                LineSmoothness = 1 // mark
            };

            srcSeries.Add(processSeries);

            return true;
        }

        private AbstractItem? GetServiceByGuid(Guid cmdGuid) => ActiveServices.FirstOrDefault(srv => srv.Guid.Equals(cmdGuid));
    }
}
