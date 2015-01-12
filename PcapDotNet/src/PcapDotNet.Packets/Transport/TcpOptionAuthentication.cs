using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Authentication Option (RFC 5925).
    /// 
    /// <para>
    /// The format of the TCP Authentication Option is:
    /// <pre>
    /// +------------+------------+------------+------------+
    /// |  Kind=29   |  Length=N  |   KeyId    | RNextKeyId |
    /// +------------+------------+------------+------------+
    /// |                     MAC           ...
    /// +-----------------------------------...
    /// 
    ///    ...-----------------+
    ///    ...  MAC (con't)    |
    ///    ...-----------------+
    /// </pre>
    /// </para>
    /// 
    /// <para>
    /// The TCP Authentication Option provides a superset of the capabilities of TCP MD5. 
    /// </para>
    /// 
    /// <para>
    /// The contents of the Message Authentication Code (MAC), are determined by the particulars of the security association. 
    /// Typical MACs are 96-128 bits (12-16 bytes), but any length that fits in the header of the segment being authenticated is allowed.
    /// </para>
    /// 
    /// </summary>
    [TcpOptionTypeRegistration(TcpOptionType.TcpAuthentication)]
    public sealed class TcpOptionAuthentication : TcpOptionComplex, IOptionComplexFactory
    {
        /// <summary>
        /// The minimum number of bytes this option takes.
        /// </summary>
        public const int OptionMinimumLength = OptionHeaderLength + 2;

        /// <summary>
        /// The minimum number of bytes this option's value takes.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        /// <summary>
        /// Creates the option using the given values.
        /// </summary>
        /// <param name="keyId">The Master Key Tuple used to generate the Message Authentication Code that authenticates this segment.</param>
        /// <param name="requestedNextKeyId">The desired Master Key Tuple that the sender would like to use to authenticate future received segments.</param>
        /// <param name="messageAuthenticationCode">The Message Authentication Code (MAC). The contents of the MAC are determined by the particulars of the security association.</param>
        public TcpOptionAuthentication(byte keyId, byte requestedNextKeyId, IList<byte> messageAuthenticationCode)
            : base(TcpOptionType.TcpAuthentication)
        {
            KeyId = keyId;
            RequestedNextKeyId = requestedNextKeyId;
            MessageAuthenticationCode = new ReadOnlyCollection<byte>(messageAuthenticationCode);
        }

        /// <summary>
        /// The default option values are zero for each key id field and no Message Authentication Code data.
        /// </summary>
        public TcpOptionAuthentication()
            : this(0, 0, new byte[0])
        {
        }

        /// <summary>
        /// The Master Key Tuple used to generate the Message Authentication Code that authenticates this segment.
        /// </summary>
        public byte KeyId { get; private set; }

        /// <summary>
        /// The desired Master Key Tuple that the sender would like to use to authenticate future received segments.
        /// </summary>
        public byte RequestedNextKeyId { get; private set; }

        /// <summary>
        /// The Message Authentication Code (MAC). The contents of the MAC are determined by the particulars of the security association. 
        /// </summary>
        public ReadOnlyCollection<byte> MessageAuthenticationCode { get; private set; }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionMinimumLength + MessageAuthenticationCode.Count; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength < OptionValueMinimumLength)
                return null;

            byte keyId = buffer[offset++];
            byte requestedNextKeyId = buffer[offset++];

            int messageAuthenticationCodeLength = valueLength - OptionValueMinimumLength;
            byte[] messageAuthenticationCode = buffer.ReadBytes(ref offset, messageAuthenticationCodeLength);
            return new TcpOptionAuthentication(keyId, requestedNextKeyId, messageAuthenticationCode);
        }

        internal override bool EqualsData(TcpOption other)
        {
            return EqualsData(other as TcpOptionAuthentication);
        }

        internal override int GetDataHashCode()
        {
            return MessageAuthenticationCode.Concat(KeyId, RequestedNextKeyId).BytesSequenceGetHashCode();
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, KeyId);
            buffer.Write(ref offset, RequestedNextKeyId);
            buffer.Write(ref offset, MessageAuthenticationCode);
        }

        private bool EqualsData(TcpOptionAuthentication other)
        {
            return other != null &&
                   KeyId.Equals(other.KeyId) &&
                   RequestedNextKeyId.Equals(other.RequestedNextKeyId) &&
                   MessageAuthenticationCode.SequenceEqual(other.MessageAuthenticationCode);
        }
    }
}
