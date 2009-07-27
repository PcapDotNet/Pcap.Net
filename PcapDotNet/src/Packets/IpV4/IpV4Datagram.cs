using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Packets
{
    /// <summary>
    /// +-----+---------+-----+-----------------+-------+-------+---------+
    /// | Bit | 0-3     | 4-7 | 8-15            | 16-18 | 19-23 | 24-31   |
    /// +-----+---------+-----+-----------------+-------+-------+---------+
    /// | 0   | Version | IHL | Type of Service | Total Length            |
    /// +-----+---------+-----+-----------------+-------+-----------------+
    /// | 32  | Identification                  | Flags | Fragment Offset |
    /// +-----+---------------+-----------------+-------+-----------------+
    /// | 64  | Time to Live  | Protocol        | Header Checksum         |
    /// +-----+---------------+-----------------+-------------------------+
    /// | 96  | Source Address                                            |
    /// +-----+-----------------------------------------------------------+
    /// | 128 | Destination Address                                       |
    /// +-----+-------------------------------------------------+---------+
    /// | 160 | Options                                         | Padding |
    /// +-----+-------------------------------------------------+---------+
    /// </summary>
    public class IpV4Datagram : Datagram
    {
        public const int HeaderMinimumLength = 20;

        internal IpV4Datagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal static void WriteHeader(byte[] buffer, int offset, IpV4TypeOfService typeOfService, ushort identification, IpV4Fragmentation fragmentation, byte ttl, IpV4Protocol protocol, IpV4Address source, IpV4Address destinationAddress, IpV4Options options)
        {
            throw new NotImplementedException();
        }
    }

    public struct IpV4TypeOfService
    {
        private byte _value;
    }

    public struct IpV4Fragmentation
    {
        private ushort _value;
    }

    public enum IpV4Protocol : byte
    {
        
    }

    public class IpV4Options
    {
        public int Length
        {
            get { throw new NotImplementedException(); }
        }
    }
}
