using MQTTnet;
using ProtoBuf;
using System;
using System.IO;
using SysMonitor.Interfaces;

namespace SysMonitor.Devices
{
    class MqttCpuServiceMonitor : IMqttMessageSender
    {
        private readonly CpuTime _cpuTime = new CpuTime();

        public MqttCpuServiceMonitor(int procId, string serviceName)
        {
            _cpuTime.Id = procId;
            _cpuTime.ServiceName = serviceName;
        }

        public MqttApplicationMessage GetMsg()
        {
            //ps -p 22534 -o %cpu 
            _cpuTime.TimePoint = DateTime.Now;
            _cpuTime.Value = 0.0f;

            if (Utils.GetCpuLoadingPercentage(out var fValue, _cpuTime.Id))
                _cpuTime.Value = fValue;

            using var stream = new MemoryStream();
            Serializer.Serialize(stream, _cpuTime);

            var cpuLoadingProtoMsg = new MqttApplicationMessageBuilder()
                .WithTopic(GetTopicName())
                .WithPayload(stream.ToArray())
                .WithAtMostOnceQoS()
                .WithRetainFlag()
                .Build();

            return cpuLoadingProtoMsg;
        }

        public DevidceType Type => DevidceType.eCPUMonitor;
        public string GetTopicName() => "RemoteSrvrData/CPU loading";
        public string GetDescription() => $"{GetTopicName()}; {_cpuTime}";
        public string GetServiceName() => _cpuTime.ServiceName;
    }
}