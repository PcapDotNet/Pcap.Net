using System;

namespace PcapDotNet.Packets
{
    [Flags]
    public enum IpV4OptionTimestampType : byte
    {
        TimestampOnly = 0,
        AddressAndTimestamp = 1,
        AddressPrespecified = 3
    }
}