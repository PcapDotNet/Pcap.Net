using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies the MTU to use on this interface. The MTU is
    /// specified as a 16-bit unsigned integer. The minimum legal value for
    /// the MTU is 68.
    /// <pre>
    ///  Code   Len      MTU
    /// +-----+-----+-----+-----+
    /// |  26 |  2  |  m1 |  m2 |
    /// +-----+-----+-----+-----+
    /// </pre>
    /// </summary>
    public class DhcpInterfaceMtuOption : DhcpUShortOption
    {
        private const ushort MIN_MTU = 68;

        /// <summary>
        /// create new DhcpInterfaceMTUOption.
        /// </summary>
        /// <param name="mtu">MTU</param>
        public DhcpInterfaceMtuOption(ushort mtu) : base(mtu, DhcpOptionCode.InterfaceMtu)
        {
            if (mtu < MIN_MTU)
                throw new ArgumentOutOfRangeException(nameof(mtu), mtu, "Minimum value of MTU is " + MIN_MTU);
        }

        internal static DhcpInterfaceMtuOption Read(DataSegment data, ref int offset)
        {
            return Read<DhcpInterfaceMtuOption>(data, ref offset, p => new DhcpInterfaceMtuOption(p));
        }

        /// <summary>
        /// RFC 2132.
        /// MTU.
        /// </summary>
        public ushort Mtu
        {
            get { return InternalValue; }
            set
            {
                if (value < MIN_MTU)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Minimum value of MTU is " + MIN_MTU);
                InternalValue = value;
            }
        }
    }
}