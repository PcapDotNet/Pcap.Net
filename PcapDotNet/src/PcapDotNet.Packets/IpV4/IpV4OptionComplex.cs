using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents a complex IPv4 option.
    /// Complex option means that it contains data and not just the type.
    /// </summary>
    public abstract class IpV4OptionComplex : IpV4Option
    {
        /// <summary>
        /// The header length in bytes for the option (type and size).
        /// </summary>
        public const int OptionHeaderLength = 2;

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
        }

        /// <summary>
        /// Constructs the option by type.
        /// </summary>
        protected IpV4OptionComplex(IpV4OptionType type)
            : base(type)
        {
        }
    }
}