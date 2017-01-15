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

        /// <summary>
        /// RFC 2131.
        /// Message op code.
        /// </summary>
        public DhcpMessageType MessageType
        {
            get { return (DhcpMessageType)this[Offset.Op]; }
        }

        /// <summary>
        /// RFC 2131.
        /// Hardware address type.
        /// </summary>
        public ArpHardwareType HardwareType
        {
            get { return (ArpHardwareType)this[Offset.Htype]; }
        }

        /// <summary>
        /// RFC 2131.
        /// Hardware address length.
        /// </summary>
        public byte HardwareAddressLength
        {
            get { return (byte)this[Offset.Hlen]; }
        }

        /// <summary>
        /// RFC 2131.
        /// Client sets to zero, optionally used by relay agents when booting via a relay agent.
        /// </summary>
        public byte Hops
        {
            get { return (byte)this[Offset.Hops]; }
        }

        /// <summary>
        /// RFC 2131.
        /// Transaction ID, a random number chosen by the client, used by the client and server to associate messages and responses between a client and a server.
        /// </summary>
        public uint TransactionId
        {
            get { return ReadUInt(Offset.Xid, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// Filled in by client, seconds elapsed since client began address acquisition or renewal process.
        /// </summary>
        public ushort SecondsElapsed
        {
            get { return ReadUShort(Offset.Secs, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// Flags.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public DhcpFlags Flags
        {
            get { return (DhcpFlags)ReadUShort(Offset.Flags, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// Client IP address; only filled in if client is in BOUND, RENEW or REBINDING state and can respond to ARP requests.
        /// </summary>
        public IpV4Address ClientIpAddress
        {
            get { return ReadIpV4Address(Offset.CiAddr, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// 'your' (client) IP address.
        /// </summary>
        public IpV4Address YourClientIpAddress
        {
            get { return ReadIpV4Address(Offset.YiAddr, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// IP address of next server to use in bootstrap; returned in DHCPOFFER, DHCPACK by server.
        /// </summary>
        public IpV4Address NextServerIpAddress
        {
            get { return ReadIpV4Address(Offset.SiAddr, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// Relay agent IP address, used in booting via a relay agent.
        /// </summary>
        public IpV4Address RelayAgentIpAddress
        {
            get { return ReadIpV4Address(Offset.GiAddr, Endianity.Big); }
        }

        /// <summary>
        /// RFC 2131.
        /// Client hardware address.
        /// </summary>
        public DataSegment ClientHardwareAddress
        {
            get { return Subsegment(Offset.ChAddr, 16); }
        }

        /// <summary>
        /// Client MAC address.
        /// </summary>
        public MacAddress ClientMacAddress
        {
            get { return new MacAddress(ReadUInt48(Offset.ChAddr, Endianity.Big)); }
        }

        /// <summary>
        /// RFC 2131.
        /// Optional server host name.
        /// </summary>
        public string ServerHostName
        {
            get
            {
                ParseServerHostName();
                return _serverHostName;
            }
        }

        /// <summary>
        /// RFC 2131.
        /// Boot file name; "generic" name or null in DHCPDISCOVER, fully qualified directory-path name in DHCPOFFER.
        /// </summary>
        public string BootFileName
        {
            get
            {
                ParseBootFileName();
                return _bootFileName;
            }
        }

        /// <summary>
        /// RFC 2131.
        /// Whether the magic dhcp-cookie is set. If set this datagram is a dhcp-datagram. Otherwise it's a bootp-datagram.
        /// </summary>
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

        /// <summary>
        /// RFC 2131.
        /// Optional parameters field.
        /// </summary>
        public IReadOnlyCollection<DhcpOption> Options
        {
            get
            {
                ParseOptions();
                return _options;
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
                DhcpFlags = Flags,
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

        
        internal DhcpDatagram(byte[] buffer, int offset, int length) : base(buffer, offset, length)
        {
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

        internal static void Write(byte[] buffer, int offset, DhcpMessageType messageType, ArpHardwareType hardwareType, byte hardwareAddressLength, byte hops, uint transactionId, ushort secondsElapsed, DhcpFlags flags, IpV4Address clientIpAddress, IpV4Address yourClientIpAddress, IpV4Address nextServerIpAddress, IpV4Address relayAgentIpAddress, DataSegment clientHardwareAddress, string serverHostName, string bootFileName, bool isDhcp, IList<DhcpOption> options)
        {
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
                    option.Write(buffer, ref offset);
                }
            }
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
                int offset = IsDhcp ? Offset.OptionsWithMagicCookie : Offset.Options;
                while (offset < Length)
                {
                    options.Add(DhcpOption.CreateInstance(this, ref offset));
                }
                _options = new ReadOnlyCollection<DhcpOption>(options);
            }
        }

        private string _serverHostName;
        private string _bootFileName;
        private IReadOnlyCollection<DhcpOption> _options;
    }
}