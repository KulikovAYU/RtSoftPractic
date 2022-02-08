using MQTTnet;
using ProtoBuf;
using System;
using System.IO;
using SysMonitor.Interfaces;

namespace SysMonitor.Devices
{
    class MqttCpuTemperatureMonitor : IMqttMessageSender
    {
        private readonly CpuTemp _cpuTemp = new CpuTemp();

        public string GetDescription() => $"{GetTopicName()}; value = {_cpuTemp.Value}; time point = {_cpuTemp.TimePoint};";
        public string GetServiceName() => "all";
        
        public MqttApplicationMessage GetMsg()
        {
            _cpuTemp.TimePoint = DateTime.Now;
            _cpuTemp.Value = 0.0f;
            if (Utils.GetCpuTemperature(out var cpuTemp))
                _cpuTemp.Value = cpuTemp;

            using MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, _cpuTemp);

            var cpuTempProtoMsg = new MqttApplicationMessageBuilder()
                .WithTopic(GetTopicName())
                .WithPayload(stream.ToArray())
                .WithAtMostOnceQoS()
                .WithRetainFlag()
                .Build();

            return cpuTempProtoMsg;
        }

        public DevidceType Type => DevidceType.eCPUTemp;
        
        public string GetTopicName() => "RemoteSrvrData/CPU temp";
    }
}