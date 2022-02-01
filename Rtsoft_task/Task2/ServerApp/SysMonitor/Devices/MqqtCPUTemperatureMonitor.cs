using MQTTnet;
using ProtoBuf;
using System;
using System.IO;

namespace SysMonitor.Devices
{

    [ProtoContract]
    class CpuTemp
    {
        [ProtoMember(1)]
        public float Value { get; set; }

        [ProtoMember(2)]
        public DateTime TimePoint { get; set; } = DateTime.Now;
    }


    class MqqtCpuTemperatureMonitor : IMqqtMessageSender
    {
        private readonly CpuTemp cpuTemp_ = new CpuTemp();

        public string GetDescription() => $"{GetTopicName()}; value = {cpuTemp_.Value}; time point = {cpuTemp_.TimePoint};";
        public string GetServiceName() => "all";
        

        public MqttApplicationMessage GetMsg()
        {
            cpuTemp_.TimePoint = DateTime.Now;
            cpuTemp_.Value = 0.0f;
            if (Utils.GetCpuTemperature(out var cpuTemp))
                cpuTemp_.Value = cpuTemp;

            using MemoryStream stream = new MemoryStream();
            Serializer.Serialize(stream, cpuTemp_);

            var cpuTempProtoMsg = new MqttApplicationMessageBuilder()
                .WithTopic(GetTopicName())
                .WithPayload(stream.ToArray())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            return cpuTempProtoMsg;
        }

        public DevidceType Type => DevidceType.eCPUTemp;
        
        public string GetTopicName() => "RemoteSrvrData/CPU temp";
    }
}