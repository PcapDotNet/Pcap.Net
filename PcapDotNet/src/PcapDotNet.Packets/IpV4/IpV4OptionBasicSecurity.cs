using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This option identifies the U.S. classification level at which the datagram is to be protected 
    /// and the authorities whose protection rules apply to each datagram.
    /// 
    /// This option is used by end systems and intermediate systems of aninternet to:
    /// 
    /// a.  Transmit from source to destination in a network standard representation the common security labels required by computer security models,
    /// 
    /// b.  Validate the datagram as appropriate for transmission from the source and delivery to the destination,
    /// 
    /// c.  Ensure that the route taken by the datagram is protected to the level required by all protection authorities indicated on the datagram.
    /// In order to provide this facility in a general Internet environment, interior and exterior gateway protocols must be augmented 
    /// to include security label information in support of routing control.
    /// 
    /// The DoD Basic Security option must be copied on fragmentation.  
    /// This option appears at most once in a datagram.  
    /// Some security systems require this to be the first option if more than one option is carried in the IP header, 
    /// but this is not a generic requirement levied by this specification.
    /// 
    /// The format of the DoD Basic Security option is as follows:
    /// +------------+------------+------------+-------------//----------+
    /// |  10000010  |  XXXXXXXX  |  SSSSSSSS  |  AAAAAAA[1]    AAAAAAA0 |
    /// |            |            |            |         [0]             |
    /// +------------+------------+------------+-------------//----------+
    ///   TYPE = 130     LENGTH   CLASSIFICATION         PROTECTION
    ///                                LEVEL              AUTHORITY
    ///                                                     FLAGS
    /// </summary>
    [IpV4OptionTypeRegistration(IpV4OptionType.BasicSecurity)]
    public class IpV4OptionBasicSecurity : IpV4OptionComplex, IIpv4OptionComplexFactory, IEquatable<IpV4OptionBasicSecurity>
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
        /// <param name="protectionAuthority">
        /// This field identifies the National Access Programs or Special Access Programs 
        /// which specify protection rules for transmission and processing of the information contained in the datagram. 
        /// </param>
        /// <param name="length">
        /// The number of bytes this option will take.
        /// </param>
        public IpV4OptionBasicSecurity(IpV4OptionSecurityClassificationLevel classificationLevel, IpV4OptionSecurityProtectionAuthority protectionAuthority, byte length)
            : base(IpV4OptionType.BasicSecurity)
        {
            if (length < OptionMinimumLength)
                throw new ArgumentOutOfRangeException("length", length, "Minimum option length is " + OptionMinimumLength);

            if (length == OptionMinimumLength && protectionAuthority != IpV4OptionSecurityProtectionAuthority.None)
            {
                throw new ArgumentException("Can't have a protection authority without minimum of " + OptionValueMinimumLength + 1 + " length",
                                            "protectionAuthority");
            }

            _classificationLevel = classificationLevel;
            _protectionAuthority = protectionAuthority;
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
            : this(classificationLevel, IpV4OptionSecurityProtectionAuthority.None, OptionMinimumLength)
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
        public IpV4OptionSecurityProtectionAuthority ProtectionAuthority
        {
            get { return _protectionAuthority; }
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
                   ProtectionAuthority == other.ProtectionAuthority &&
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
        public override int GetHashCode()
        {
            return base.GetHashCode() ^
                   ((((byte)ClassificationLevel) << 16) | (((byte)ProtectionAuthority) << 8) | Length).GetHashCode();
        }

        public IpV4OptionComplex CreateInstance(byte[] buffer, ref int offset, byte valueLength)
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

            // Protection authority
            int protectionAuthorityLength = valueLength - OptionValueMinimumLength;
            IpV4OptionSecurityProtectionAuthority protectionAuthority = IpV4OptionSecurityProtectionAuthority.None;
            if (protectionAuthorityLength > 0)
            {
                for (int i = 0; i < protectionAuthorityLength - 1; ++i)
                {
                    if ((buffer[offset + i] & 0x01) == 0)
                        return null;
                }
                if ((buffer[offset + protectionAuthorityLength - 1] & 0x01) != 0)
                    return null;

                protectionAuthority = (IpV4OptionSecurityProtectionAuthority)(buffer[offset] & 0xFE);
            }
            offset += protectionAuthorityLength;

            return new IpV4OptionBasicSecurity(classificationLevel, protectionAuthority, (byte)(OptionMinimumLength + protectionAuthorityLength));
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)ClassificationLevel;

            int protectionAuthorityLength = Length - OptionMinimumLength;
            if (protectionAuthorityLength > 0)
            {
                buffer[offset++] = (byte)ProtectionAuthority;
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
        private readonly IpV4OptionSecurityProtectionAuthority _protectionAuthority;
        private readonly byte _length;
    }
}