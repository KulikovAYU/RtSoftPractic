using System;
using ProtoBuf;

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

        public override string ToString() => $"value = {Value}; time point = {TimePoint}; service = {ServiceName}; Id = {Id}";
    }
}