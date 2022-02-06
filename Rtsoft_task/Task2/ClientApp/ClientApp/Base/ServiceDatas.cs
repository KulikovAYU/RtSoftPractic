using System;
using ProtoBuf;

namespace ClientApp.Base
{
    [ProtoContract]
    public class CpuTime
    {
        [ProtoMember(1)]
        public float Value { get; set; }

        [ProtoMember(2)]
        public DateTime TimePoint { get; set; }

        [ProtoMember(3)]
        public string ServiceName { get; set; }

        public override string ToString()
        {
            return $"Value = {Value}; TimePoint = {TimePoint}; ServiceName = {ServiceName}";
        }
    }

    [ProtoContract]
    public class CpuTemp
    {
        [ProtoMember(1)]
        public float Value { get; set; }

        [ProtoMember(2)]
        public DateTime TimePoint { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"Value = {Value}; TimePoint = {TimePoint}";
        }
    }
}
