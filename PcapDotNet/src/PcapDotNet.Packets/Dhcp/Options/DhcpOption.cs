using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// Abstract class for all possible Dhcp-Options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public abstract class DhcpOption : IEquatable<DhcpOption>
    {
        /// <summary>
        /// Option-Code according RFC 2132.
        /// </summary>
        public DhcpOptionCode OptionCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public abstract byte Length
        {
            get;
        }

        /// <summary>
        /// create new Option.
        /// </summary>
        /// <param name="code">Option-Code</param>
        protected DhcpOption(DhcpOptionCode code)
        {
            OptionCode = code;
        }
        
        internal virtual void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(offset++, (byte)OptionCode);
            buffer.Write(offset++, Length);
        }

        /// <summary>
        /// Two options objects are equal if they have the same parameters.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DhcpOption);
        }

        /// <summary>
        /// Two options objects are equal if they have the same parameters.
        /// </summary>
        public bool Equals(DhcpOption other)
        {
            if (other == null)
                return false;

            if (OptionCode != other.OptionCode)
                return false;

            if (Length != other.Length)
                return false;

            //we compare the output of write

            byte[] selfData = new byte[2 + Length];
            byte[] otherData = new byte[2 + other.Length];
            int offset = 0;
            Write(selfData, ref offset);
            offset = 0;
            other.Write(otherData, ref offset);

            return selfData.SequenceEqual(otherData);
        }

        /// <summary>
        /// calculate a hash of the option.
        /// </summary>
        /// <returns>a hash representing this instance</returns>
        public override int GetHashCode()
        {
            byte[] selfData = new byte[2 + Length];
            int offset = 0;
            Write(selfData, ref offset);

            return Sequence.GetHashCode(selfData.Cast<object>().ToArray());
        }
    }
}