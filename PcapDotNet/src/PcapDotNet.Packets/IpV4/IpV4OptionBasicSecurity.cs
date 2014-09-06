using System;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This option identifies the U.S. classification level at which the datagram is to be protected 
    /// and the authorities whose protection rules apply to each datagram.
    /// 
    /// <para>
    ///   This option is used by end systems and intermediate systems of an internet to:
    ///   <list type="number">
    ///     <item>Transmit from source to destination in a network standard representation the common security labels required by computer security models.</item>
    ///     <item>Validate the datagram as appropriate for transmission from the source and delivery to the destination.</item>
    ///     <item>
    ///       Ensure that the route taken by the datagram is protected to the level required by all protection authorities indicated on the datagram.
    ///       In order to provide this facility in a general Internet environment, interior and exterior gateway protocols must be augmented 
    ///       to include security label information in support of routing control.
    ///     </item>
    ///   </list>
    /// </para>
    ///
    /// <para>
    ///   The DoD Basic Security option must be copied on fragmentation.  
    ///   This option appears at most once in a datagram.  
    ///   Some security systems require this to be the first option if more than one option is carried in the IP header, 
    ///   but this is not a generic requirement levied by this specification.
    /// </para>
    /// 
    /// <para>
    ///   The format of the DoD Basic Security option is as follows:
    ///   <pre>
    /// +------------+------------+------------+-------------//----------+
    /// |  10000010  |  XXXXXXXX  |  SSSSSSSS  |  AAAAAAA[1]    AAAAAAA0 |
    /// |            |            |            |         [0]             |
    /// +------------+------------+------------+-------------//----------+
    ///   TYPE = 130     LENGTH   CLASSIFICATION         PROTECTION
    ///                                LEVEL              AUTHORITY
    ///                                                     FLAGS
    ///   </pre>
    /// </para>
    /// </summary>
    [IpV4OptionTypeRegistration(IpV4OptionType.BasicSecurity)]
    public sealed class IpV4OptionBasicSecurity : IpV4OptionComplex, IOptionComplexFactory, IEquatable<IpV4OptionBasicSecurity>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = 3;

        /// <summary>
        /// The minimum number of bytes this option's value take.
        /// </summary>
        public const int OptionValueMinimumLength = OptionMinimumLength - OptionHeaderLength;

        /// <summary>
        /// Create the security option from the different security field values.
        /// </summary>
        /// <param name="classificationLevel">
        /// This field specifies the (U.S.) classification level at which the datagram must be protected.  
        /// The information in the datagram must be protected at this level.  
        /// </param>
        /// <param name="protectionAuthorities">
        /// This field identifies the National Access Programs or Special Access Programs 
        /// which specify protection rules for transmission and processing of the information contained in the datagram. 
        /// </param>
        /// <param name="length">
        /// The number of bytes this option will take.
        /// </param>
        public IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel classificationLevel, IpV4OptionSecurityProtectionAuthorities protectionAuthorities, byte length)
            : base(IpV4OptionType.BasicSecurity)
        {
            if (length < OptionMinimumLength)
                throw new ArgumentOutOfRangeException("length", length, "Minimum option length is " + OptionMinimumLength);

            if (length == OptionMinimumLength && protectionAuthorities != IpV4OptionSecurityProtectionAuthorities.None)
            {
                throw new ArgumentException("Can't have a protection authority without minimum of " + (OptionValueMinimumLength + 1) + " length",
                                            "protectionAuthorities");
            }

            if (classificationLevel != IpV4OptionSecurityClassificationLevel.Confidential &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.Secret &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.TopSecret &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.Unclassified)
            {
                throw new ArgumentException("Invalid classification level " + classificationLevel);
            }

            _classificationLevel = classificationLevel;
            _protectionAuthorities = protectionAuthorities;
            _length = length;
        }

        /// <summary>
        /// Create the security option with only classification level.
        /// </summary>
        /// <param name="classificationLevel">
        /// This field specifies the (U.S.) classification level at which the datagram must be protected.  
        /// The information in the datagram must be protected at this level.  
        /// </param>
        public IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel classificationLevel)
            : this(classificationLevel, IpV4OptionSecurityProtectionAuthorities.None, OptionMinimumLength)
        {
        }

        /// <summary>
        /// Creates unclassified security option.
        /// </summary>
        public IpV4OptionBasicSecurity()
            : this(IpV4OptionSecurityClassificationLevel.Unclassified)
        {
        }

        /// <summary>
        /// This field specifies the (U.S.) classification level at which the datagram must be protected.  
        /// The information in the datagram must be protected at this level.  
        /// </summary>
        public IpV4OptionSecurityClassificationLevel ClassificationLevel
        {
            get { return _classificationLevel; }
        }

        /// <summary>
        /// This field identifies the National Access Programs or Special Access Programs 
        /// which specify protection rules for transmission and processing of the information contained in the datagram. 
        /// </summary>
        public IpV4OptionSecurityProtectionAuthorities ProtectionAuthorities
        {
            get { return _protectionAuthorities; }
        }


        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return true; }
        }

        /// <summary>
        /// Two security options are equal iff they have the exact same field values.
        /// </summary>
        public bool Equals(IpV4OptionBasicSecurity other)
        {
            if (other == null)
                return false;

            return ClassificationLevel == other.ClassificationLevel &&
                   ProtectionAuthorities == other.ProtectionAuthorities &&
                   Length == other.Length;
        }

        /// <summary>
        /// Two security options are equal iff they have the exact same field values.
        /// </summary>
        public override bool Equals(IpV4Option other)
        {
            return Equals(other as IpV4OptionBasicSecurity);
        }

        /// <summary>
        /// The hash code is the xor of the base class hash code 
        /// with the hash code of the combination of the classification level, protection authority and length.
        /// </summary>
        /// <returns></returns>
        internal override int GetDataHashCode()
        {
            return BitSequence.Merge((byte)ClassificationLevel, (byte)ProtectionAuthorities,(byte)Length).GetHashCode();
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

            // Classification level
            IpV4OptionSecurityClassificationLevel classificationLevel = (IpV4OptionSecurityClassificationLevel)buffer[offset++];
            if (classificationLevel != IpV4OptionSecurityClassificationLevel.Confidential &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.Secret &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.TopSecret &&
                classificationLevel != IpV4OptionSecurityClassificationLevel.Unclassified)
            {
                return null;
            }

            // Protection authorities
            int protectionAuthoritiesLength = valueLength - OptionValueMinimumLength;
            IpV4OptionSecurityProtectionAuthorities protectionAuthorities = IpV4OptionSecurityProtectionAuthorities.None;
            if (protectionAuthoritiesLength > 0)
            {
                for (int i = 0; i < protectionAuthoritiesLength - 1; ++i)
                {
                    if ((buffer[offset + i] & 0x01) == 0)
                        return null;
                }
                if ((buffer[offset + protectionAuthoritiesLength - 1] & 0x01) != 0)
                    return null;

                protectionAuthorities = (IpV4OptionSecurityProtectionAuthorities)(buffer[offset] & 0xFE);
            }
            offset += protectionAuthoritiesLength;

            return new IpV4OptionBasicSecurity(classificationLevel, protectionAuthorities, (byte)(OptionMinimumLength + protectionAuthoritiesLength));
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)ClassificationLevel;

            int protectionAuthorityLength = Length - OptionMinimumLength;
            if (protectionAuthorityLength > 0)
            {
                buffer[offset++] = (byte)ProtectionAuthorities;
                if (protectionAuthorityLength > 1)
                {
                    buffer[offset - 1] |= 0x01;
                    for (int i = 0; i != protectionAuthorityLength - 2; ++i)
                        buffer[offset++] = 0x01;
                    buffer[offset++] = 0x00;
                }
            }
        }

        private readonly IpV4OptionSecurityClassificationLevel _classificationLevel;
        private readonly IpV4OptionSecurityProtectionAuthorities _protectionAuthorities;
        private readonly byte _length;
    }
}