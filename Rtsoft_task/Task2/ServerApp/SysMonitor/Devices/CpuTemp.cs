using System;
using ProtoBuf;

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
}