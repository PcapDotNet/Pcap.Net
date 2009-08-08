using System;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// The Router Alert option has the semantic "routers should examine this packet more closely".  
    /// By including the Router Alert option in the IP header of its protocol message, 
    /// RSVP can cause the message to be intercepted while causing little or no performance 
    /// penalty on the forwarding of normal data packets.
    /// 
    /// Routers that support option processing in the fast path already demultiplex processing based on the option type field.  
    /// If all option types are supported in the fast path, then the addition of another option type to process is unlikely to impact performance.  
    /// If some option types are not supported in the fast path, 
    /// this new option type will be unrecognized and cause packets carrying it to be kicked out into the slow path, 
    /// so no change to the fast path is necessary, and no performance penalty will be incurred for regular data packets.
    /// 
    /// Routers that do not support option processing in the fast path will cause packets carrying this new option 
    /// to be forwarded through the slow path, so no change to the fast path is necessary and no performance penalty 
    /// will be incurred for regular data packets.
    /// 
    /// The Router Alert option has the following format:
    /// +--------+--------+--------+--------+
    /// |10010100|00000100|  2 octet value  |
    /// +--------+--------+--------+--------+
    /// </summary>
    [IpV4OptionTypeRegistration(IpV4OptionType.RouterAlert)]
    public class IpV4OptionRouterAlert : IpV4OptionComplex, IIpv4OptionComplexFactory, IEquatable<IpV4OptionRouterAlert>
    {
        /// <summary>
        /// The number of bytes this option take.
        /// </summary>
        public const int OptionLength = 4;

        /// <summary>
        /// The number of bytes this option's value take.
        /// </summary>
        public const int OptionValueLength = OptionLength - OptionHeaderLength;

        /// <summary>
        /// Create the option according to the given value.
        /// </summary>
        public IpV4OptionRouterAlert(ushort value)
            : base(IpV4OptionType.RouterAlert)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a 0 value router alert option
        /// </summary>
        public IpV4OptionRouterAlert()
            : this(0)
        {
        }

        /// <summary>
        /// The value of the alert.
        /// </summary>
        public ushort Value
        {
            get { return _value; }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionLength; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two stream identifier options are equal if they have the same identifier.
        /// </summary>
        public bool Equals(IpV4OptionRouterAlert other)
        {
            if (other == null)
                return false;
            return Value == other.Value;
        }

        /// <summary>
        /// Two stream identifier options are equal if they have the same identifier.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionRouterAlert);
        }

        /// <summary>
        /// The hash code value is the xor of the base class hash code and the value hash code.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   Value.GetHashCode();
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        public IpV4OptionComplex CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength != OptionHeaderLength)
                return null;

            ushort value = buffer.ReadUShort(ref offset, Endianity.Big);
            return new IpV4OptionRouterAlert(value);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Value, Endianity.Big);
        }

        private readonly ushort _value;
    }
}