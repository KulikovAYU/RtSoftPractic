using MQTTnet;
using ProtoBuf;
using System;
using System.IO;

namespace SysMonitor.Devices
{
    [ProtoContract]
    class CpuTime
    {
        [ProtoMember(1)]
        public float Value { get; set; }

        [ProtoMember(2)]
        public DateTime TimePoint { get; set; } = DateTime.Now;

        [ProtoMember(3)]
        public string ServiceName { get; set; }

        [ProtoMember(4)]
        public int Id { get; set; }

        public override string ToString()
        {
            return $"value = {Value}; time point = {TimePoint}; service = {ServiceName}; Id = {Id}";
        }
    }

    class MqqtCpuServiceMonitor : IMqqtMessageSender
    {
        private readonly CpuTime cpuTime_ = new CpuTime();

        public MqqtCpuServiceMonitor(int procId, string serviceName)
        {
            cpuTime_.Id = procId;
            cpuTime_.ServiceName = serviceName;
        }

        public MqttApplicationMessage GetMsg()
        {
            //ps -p 22534 -o %cpu 
            cpuTime_.TimePoint = DateTime.Now;
            cpuTime_.Value = 0.0f;

            if (Utils.GetCpuLoadingPercentage(out var fValue, cpuTime_.Id))
                cpuTime_.Value = fValue;

            using var stream = new MemoryStream();
            Serializer.Serialize(stream, cpuTime_);

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

        public string GetDescription() => $"{GetTopicName()}; {cpuTime_}";
        public string GetServiceName() => cpuTime_.ServiceName;
    }
}