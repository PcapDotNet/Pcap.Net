using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// <pre>
    /// +-----+------+------+-----------+
    /// | Bit | 0-7  | 8-15 | 16-31     |
    /// +-----+------+------+-----------+
    /// | 0   | Type | Code | Checksum  |
    /// +-----+------+------+-----------+
    /// | 32  | unused                  |
    /// +-----+-------------------------+
    /// | 64  | IpV4 datagram           |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public abstract class IcmpIpV4PayloadDatagram : IcmpDatagram
    {
        internal IcmpIpV4PayloadDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        /// <summary>
        /// The ICMP payload as an IPv4 datagram.
        /// </summary>
        public IpV4Datagram IpV4
        {
            get
            {
                if (_ipV4 == null && Length >= HeaderLength)
                {
                    if (IsIpV4PayloadLimited)
                    {
                        int ipV4HeaderLength = IpV4Datagram.GetHeaderLength(Subsegment(HeaderLength, Length - HeaderLength));
                        _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Math.Min(Length - HeaderLength, ipV4HeaderLength + IpV4PayloadLimit));
                    }
                    else
                    {
                        _ipV4 = new IpV4Datagram(Buffer, StartOffset + HeaderLength, Length - HeaderLength);
                    }
                }
                return _ipV4;
            }
        }

        /// <summary>
        /// ICMP with IPv4 payload is valid if the datagram's length is OK, the checksum is correct, the code is in the expected range,
        /// and the IPv4 payload contains at least an IPv4 header.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            if (!base.CalculateIsValid())
                return false;

            IpV4Datagram ip = IpV4;
            return (ip.Length >= IpV4Datagram.HeaderMinimumLength && ip.Length >= ip.HeaderLength);
        }

        internal virtual int IpV4PayloadLimit
        {
            get { throw new NotImplementedException(); }
        }

        internal virtual bool IsIpV4PayloadLimited
        {
            get { return false; }
        }

        private IpV4Datagram _ipV4;
    }
}