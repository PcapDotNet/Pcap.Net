using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.Arp;
using PcapDotNet.Packets.Dhcp.Options;
using PcapDotNet.Packets.Ethernet;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp
{
    /// <summary>
    /// RFC 2131.
    /// The DHCP message structure is shown below:
    /// <pre>
    /// 0                   1                   2                   3
    /// 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |     op (1)    |   htype (1)   |   hlen (1)    |   hops (1)    |
    /// +---------------+---------------+---------------+---------------+
    /// |                            xid (4)                            |
    /// +-------------------------------+-------------------------------+
    /// |           secs (2)            |           flags (2)           |
    /// +-------------------------------+-------------------------------+
    /// |                          ciaddr  (4)                          |
    /// +---------------------------------------------------------------+
    /// |                          yiaddr  (4)                          |
    /// +---------------------------------------------------------------+
    /// |                          siaddr  (4)                          |
    /// +---------------------------------------------------------------+
    /// |                          giaddr  (4)                          |
    /// +---------------------------------------------------------------+
    /// |                                                               |
    /// |                          chaddr  (16)                         |
    /// |                                                               |
    /// |                                                               |
    /// +---------------------------------------------------------------+
    /// |                                                               |
    /// |                          sname   (64)                         |
    /// +---------------------------------------------------------------+
    /// |                                                               |
    /// |                          file    (128)                        |
    /// +---------------------------------------------------------------+
    /// |                                                               |
    /// |                          options (variable)                   |
    /// +---------------------------------------------------------------+
    /// </pre>
    /// </summary>
    public class DhcpDatagram : Datagram
    {
        private static class Offset
        {
            public const int Op = 0;
            public const int Htype = 1;
            public const int Hlen = 2;
            public const int Hops = 3;
            public const int Xid = 4;
            public const int Secs = 8;
            public const int Flags = 10;
            public const int CiAddr = 12;
            public const int YiAddr = 16;
            public const int SiAddr = 20;
            public const int GiAddr = 24;
            public const int ChAddr = 28;
            public const int Sname = 44;
            public const int File = 108;
            public const int Options = 236;
            public const int OptionsWithMagicCookie = 240;
        }

        internal const int DHCP_MAGIC_COOKIE = 0x63825363;

        public DhcpMessageType MessageType
        {
            get { return (DhcpMessageType)this[Offset.Op]; }
        }

        public ArpHardwareType HardwareType
        {
            get { return (ArpHardwareType)this[Offset.Htype]; }
        }

        public byte HardwareAddressLength
        {
            get { return (byte)this[Offset.Hlen]; }
        }

        public byte Hops
        {
            get { return (byte)this[Offset.Hops]; }
        }

        public int TransactionId
        {
            get { return ReadInt(Offset.Xid, Endianity.Big); }
        }

        public ushort SecondsElapsed
        {
            get { return ReadUShort(Offset.Secs, Endianity.Big); }
        }

        public DhcpFlags Flags
        {
            get { return (DhcpFlags)ReadUShort(Offset.Flags, Endianity.Big); }
        }

        public IpV4Address ClientIpAddress
        {
            get { return ReadIpV4Address(Offset.CiAddr, Endianity.Big); }
        }

        public IpV4Address YourClientIpAddress
        {
            get { return ReadIpV4Address(Offset.YiAddr, Endianity.Big); }
        }

        public IpV4Address NextServerIpAddress
        {
            get { return ReadIpV4Address(Offset.SiAddr, Endianity.Big); }
        }

        public IpV4Address RelayAgentIpAddress
        {
            get { return ReadIpV4Address(Offset.GiAddr, Endianity.Big); }
        }

        public DataSegment ClientHardwareAddress
        {
            get { return this.Subsegment(Offset.ChAddr, 16); }
        }

        public MacAddress ClientMacAddress
        {
            get { return new MacAddress(ReadUInt48(Offset.ChAddr, Endianity.Big)); }
        }

        public string ServerHostName
        {
            get
            {
                ParseServerHostName();
                return _serverHostName;
            }
        }

        public string BootFileName
        {
            get
            {
                ParseBootFileName();
                return _bootFileName;
            }
        }

        public bool IsDhcp
        {
            get
            {
                if (Length >= Offset.Options + 4)
                {
                    return ReadInt(Offset.Options, Endianity.Big) == DHCP_MAGIC_COOKIE;
                }
                else
                {
                    return false;
                }
            }
        }

        public IReadOnlyCollection<DhcpOption> Options
        {
            get
            {
                ParseOptions();
                return _options;
            }
        }

        internal DhcpDatagram(byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
        }

        private void ParseServerHostName()
        {
            if (_serverHostName == null)
            {
                //at the moment we only interpret server host name as sname and ignore options
                byte[] byteServerHostName = ReadBytes(Offset.Sname, 64);
                _serverHostName = Encoding.ASCII.GetString(byteServerHostName).TrimEnd('\0');
            }
        }

        private void ParseBootFileName()
        {
            if (_bootFileName == null)
            {
                //at the moment we only interpret server host name as sname and ignore options
                byte[] byteBootFileName = ReadBytes(Offset.File, 128);
                _bootFileName = Encoding.ASCII.GetString(byteBootFileName).TrimEnd('\0');
            }
        }

        private void ParseOptions()
        {
            if (_options == null)
            {
                List<DhcpOption> options = new List<DhcpOption>();
                int pos = IsDhcp ? Offset.OptionsWithMagicCookie : Offset.Options;
                while (pos < Length)
                {
                    options.Add(DhcpOption.CreateInstance(this, ref pos));
                }
                this._options = new ReadOnlyCollection<DhcpOption>(options);
            }
        }

        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new DhcpLayer
            {
                MessageType = MessageType,
                HardwareType = HardwareType,
                HardwareAddressLength = HardwareAddressLength,
                Hops = Hops,
                TransactionId = TransactionId,
                SecondsElapsed = SecondsElapsed,
                Flags = Flags,
                ClientIpAddress = ClientIpAddress,
                YourClientIpAddress = YourClientIpAddress,
                NextServerIpAddress = NextServerIpAddress,
                RelayAgentIpAddress = RelayAgentIpAddress,
                ClientHardwareAddress = ClientHardwareAddress,
                ServerHostName = ServerHostName,
                BootFileName = BootFileName,
                IsDhcp = IsDhcp,
                Options = Options.ToList()
            };
        }

        internal static int GetLength(bool isDhcp, IList<DhcpOption> options)
        {
            int length = isDhcp ? Offset.OptionsWithMagicCookie : Offset.Options;

            if (options != null)
            {
                length += options.Sum(p => 1 + (!(p is DhcpPadOption || p is DhcpEndOption) ? 1 : 0) + p.Length); //Type + Len? + Option
            }

            return length;
        }

        internal static void Write(byte[] buffer, int offset, DhcpMessageType messageType, ArpHardwareType hardwareType, byte hardwareAddressLength, byte hops, int transactionId, ushort secondsElapsed, DhcpFlags flags, IpV4Address clientIpAddress, IpV4Address yourClientIpAddress, IpV4Address nextServerIpAddress, IpV4Address relayAgentIpAddress, DataSegment clientHardwareAddress, string serverHostName, string bootFileName, bool isDhcp, IList<DhcpOption> options)
        {
            int startOffset = offset;
            buffer.Write(ref offset, (byte)messageType);
            buffer.Write(ref offset, (byte)hardwareType);
            buffer.Write(ref offset, hardwareAddressLength);
            buffer.Write(ref offset, hops);
            buffer.Write(ref offset, transactionId, Endianity.Big);
            buffer.Write(ref offset, secondsElapsed, Endianity.Big);
            buffer.Write(ref offset, (ushort)flags, Endianity.Big);
            buffer.Write(ref offset, clientIpAddress, Endianity.Big);
            buffer.Write(ref offset, yourClientIpAddress, Endianity.Big);
            buffer.Write(ref offset, nextServerIpAddress, Endianity.Big);
            buffer.Write(ref offset, relayAgentIpAddress, Endianity.Big);
            buffer.Write(offset, clientHardwareAddress);
            offset += Offset.Sname - Offset.ChAddr;
            if (serverHostName != null)
            {
                buffer.Write(offset, new DataSegment(Encoding.ASCII.GetBytes(serverHostName)));
            }
            offset += Offset.File - Offset.Sname;
            if (bootFileName != null)
            {
                buffer.Write(offset, new DataSegment(Encoding.ASCII.GetBytes(bootFileName)));
            }
            offset += Offset.Options - Offset.File;
            if (isDhcp)
            {
                buffer.Write(ref offset, DHCP_MAGIC_COOKIE, Endianity.Big);
            }
            if (options != null)
            {
                foreach (DhcpOption option in options)
                {
                    startOffset = offset;
                    option.Write(buffer, ref offset);
                    if (offset != startOffset + option.Length + 2)
                    {
                        if (!(option is DhcpPadOption || option is DhcpEndOption))
                        {
                            Console.WriteLine("fail2");
                        }
                    }
                }
            }
        }

        private string _serverHostName;
        private string _bootFileName;
        private IReadOnlyCollection<DhcpOption> _options;
    }
}