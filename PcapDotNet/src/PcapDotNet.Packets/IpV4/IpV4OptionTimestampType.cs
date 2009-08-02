using System;

namespace PcapDotNet.Packets.IpV4
{
    public enum IpV4OptionTimestampType : byte
    {
        TimestampOnly = 0,
        AddressAndTimestamp = 1,
        AddressPrespecified = 3
    }
}