using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2065, 2535.
    /// <pre>
    /// +-----+---+---+--------------+----+----------+------+--------+-------+-------+----------+-------+
    /// | bit | 0 | 1 | 2            | 3  | 4        | 5    | 6-7    | 8     | 9     | 10-11    | 12-15 |
    /// +-----+---+---+--------------+----+----------+------+--------+-------+-------+----------+-------+
    /// | 0   | A | C | experimental | XT | Reserved | user | NAMTYP | IPSEC | email | Reserved | SIG   |
    /// +-----+---+---+--------------+----+----------+------+--------+-------+-------+----------+-------+
    /// | 16  | protocol                                             | algorithm                        |
    /// +-----+------------------------------------------------------+----------------------------------+
    /// | 32  | Flags extension (optional)                                                              |
    /// +-----+-----------------------------------------------------------------------------------------+
    /// | 32  | public key                                                                              |
    /// | or  |                                                                                         |
    /// | 48  |                                                                                         |
    /// | ... |                                                                                         |
    /// +-----+-----------------------------------------------------------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Key)]
    public sealed class DnsResourceDataKey : DnsResourceDataSimple, IEquatable<DnsResourceDataKey>
    {
        private static class Offset
        {
            public const int AuthenticationProhibited = 0;
            public const int ConfidentialityProhibited = 0;
            public const int Experimental = 0;
            public const int IsFlagsExtension = 0;
            public const int UserAssociated = 0;
            public const int NameType = 0;
            public const int IpSec = 1;
            public const int Email = 1;
            public const int Signatory = 1;
            public const int Protocol = sizeof(ushort);
            public const int Algorithm = Protocol + sizeof(byte);
            public const int FlagsExtension = Algorithm + sizeof(byte);
        }

        private static class Mask
        {
            public const byte AuthenticationProhibited = 0x80;
            public const byte ConfidentialityProhibited = 0x40;
            public const byte Experimental = 0x20;
            public const byte IsFlagsExtension = 0x10;
            public const byte UserAssociated = 0x04;
            public const byte IpSec = 0x80;
            public const byte Email = 0x40;
            public const byte NameType = 0x03;
            public const byte Signatory = 0x0F;
        }

        private const int ConstantPartLength = Offset.FlagsExtension;

        /// <summary>
        /// Constructs an instance out of the authentication prohibited, confidentiality prohibited, experimental, user associated, IPSec, email, name type, 
        /// signatory, protocol, algorithm, flags extension and public key fields.
        /// </summary>
        /// <param name="authenticationProhibited">Use of the key is prohibited for authentication.</param>
        /// <param name="confidentialityProhibited">Use of the key is prohibited for confidentiality.</param>
        /// <param name="experimental">
        /// Ignored if the type field indicates "no key" and the following description assumes that type field to be non-zero.
        /// Keys may be associated with zones, entities, or users for experimental, trial, or optional use, in which case this bit will be one.
        /// If this bit is a zero, it means that the use or availability of security based on the key is "mandatory". 
        /// Thus, if this bit is off for a zone key, the zone should be assumed secured by SIG RRs and any responses indicating the zone is not secured should be considered bogus.
        /// If this bit is a one for a host or end entity, it might sometimes operate in a secure mode and at other times operate without security.
        /// The experimental bit, like all other aspects of the KEY RR, is only effective if the KEY RR is appropriately signed by a SIG RR.
        /// The experimental bit must be zero for safe secure operation and should only be a one for a minimal transition period.
        /// </param>
        /// <param name="userAssociated">
        /// Indicates that this is a key associated with a "user" or "account" at an end entity, usually a host.
        /// The coding of the owner name is that used for the responsible individual mailbox in the SOA and RP RRs:
        /// The owner name is the user name as the name of a node under the entity name.
        /// For example, "j.random_user" on host.subdomain.domain could have a public key associated through a KEY RR
        /// with name j\.random_user.host.subdomain.domain and the user bit a one.
        /// It could be used in an security protocol where authentication of a user was desired.
        /// This key might be useful in IP or other security for a user level service such a telnet, ftp, rlogin, etc.
        /// </param>
        /// <param name="ipSec">
        /// Indicates that this key is valid for use in conjunction with that security standard.
        /// This key could be used in connection with secured communication on behalf of an end entity or user whose name is the owner name of the KEY RR
        /// if the entity or user bits are on.
        /// The presence of a KEY resource with the IPSEC and entity bits on and experimental and no-key bits off is an assertion that the host speaks IPSEC.
        /// </param>
        /// <param name="email">
        /// Indicates that this key is valid for use in conjunction with MIME security multiparts.
        /// This key could be used in connection with secured communication on behalf of an end entity or user
        /// whose name is the owner name of the KEY RR if the entity or user bits are on.
        /// </param>
        /// <param name="nameType">The name type.</param>
        /// <param name="signatory">
        /// If non-zero, indicates that the key can validly sign things as specified in DNS dynamic update.
        /// Note that zone keys always have authority to sign any RRs in the zone regardless of the value of the signatory field.
        /// </param>
        /// <param name="protocol">
        /// It is anticipated that keys stored in DNS will be used in conjunction with a variety of Internet protocols.
        /// It is intended that the protocol octet and possibly some of the currently unused (must be zero) bits in the KEY RR flags 
        /// as specified in the future will be used to indicate a key's validity for different protocols.
        /// </param>
        /// <param name="algorithm">The key algorithm parallel to the same field for the SIG resource.</param>
        /// <param name="flagsExtension">
        /// Optional second 16 bit flag field after the algorithm octet and before the key data.
        /// Must not be non-null unless one or more such additional bits have been defined and are non-zero.
        /// </param>
        /// <param name="publicKey">The public key value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags")]
        public DnsResourceDataKey(bool authenticationProhibited, bool confidentialityProhibited, bool experimental, bool userAssociated, bool ipSec,
                                  bool email, DnsKeyNameType nameType, DnsKeySignatoryAttributes signatory, DnsKeyProtocol protocol, DnsAlgorithm algorithm,
                                  ushort? flagsExtension, DataSegment publicKey)
        {
            AuthenticationProhibited = authenticationProhibited;
            ConfidentialityProhibited = confidentialityProhibited;
            Experimental = experimental;
            UserAssociated = userAssociated;
            IpSec = ipSec;
            Email = email;
            FlagsExtension = flagsExtension;
            NameType = nameType;
            Signatory = signatory;
            Protocol = protocol;
            Algorithm = algorithm;
            PublicKey = publicKey;
        }

        /// <summary>
        /// Use of the key is prohibited for authentication.
        /// </summary>
        public bool AuthenticationProhibited { get; private set; }

        /// <summary>
        /// Use of the key is prohibited for confidentiality.
        /// </summary>
        public bool ConfidentialityProhibited { get; private set; }

        /// <summary>
        /// Ignored if the type field indicates "no key" and the following description assumes that type field to be non-zero.
        /// Keys may be associated with zones, entities, or users for experimental, trial, or optional use, in which case this bit will be one.
        /// If this bit is a zero, it means that the use or availability of security based on the key is "mandatory". 
        /// Thus, if this bit is off for a zone key, the zone should be assumed secured by SIG RRs and any responses indicating the zone is not secured should be considered bogus.
        /// If this bit is a one for a host or end entity, it might sometimes operate in a secure mode and at other times operate without security.
        /// The experimental bit, like all other aspects of the KEY RR, is only effective if the KEY RR is appropriately signed by a SIG RR.
        /// The experimental bit must be zero for safe secure operation and should only be a one for a minimal transition period.
        /// </summary>
        public bool Experimental { get; private set; }

        /// <summary>
        /// Indicates that this is a key associated with a "user" or "account" at an end entity, usually a host.
        /// The coding of the owner name is that used for the responsible individual mailbox in the SOA and RP RRs:
        /// The owner name is the user name as the name of a node under the entity name.
        /// For example, "j.random_user" on host.subdomain.domain could have a public key associated through a KEY RR
        /// with name j\.random_user.host.subdomain.domain and the user bit a one.
        /// It could be used in an security protocol where authentication of a user was desired.
        /// This key might be useful in IP or other security for a user level service such a telnet, ftp, rlogin, etc.
        /// </summary>
        public bool UserAssociated { get; private set; }

        /// <summary>
        /// Indicates that this key is valid for use in conjunction with that security standard.
        /// This key could be used in connection with secured communication on behalf of an end entity or user whose name is the owner name of the KEY RR
        /// if the entity or user bits are on.
        /// The presence of a KEY resource with the IPSEC and entity bits on and experimental and no-key bits off is an assertion that the host speaks IPSEC.
        /// </summary>
        public bool IpSec { get; private set; }

        /// <summary>
        /// Indicates that this key is valid for use in conjunction with MIME security multiparts.
        /// This key could be used in connection with secured communication on behalf of an end entity or user
        /// whose name is the owner name of the KEY RR if the entity or user bits are on.
        /// </summary>
        public bool Email { get; private set; }

        /// <summary>
        /// The name type.
        /// </summary>
        public DnsKeyNameType NameType { get; private set; }

        /// <summary>
        /// If non-zero, indicates that the key can validly sign things as specified in DNS dynamic update.
        /// Note that zone keys always have authority to sign any RRs in the zone regardless of the value of the signatory field.
        /// </summary>
        public DnsKeySignatoryAttributes Signatory { get; private set; }

        /// <summary>
        /// It is anticipated that keys stored in DNS will be used in conjunction with a variety of Internet protocols.
        /// It is intended that the protocol octet and possibly some of the currently unused (must be zero) bits in the KEY RR flags 
        /// as specified in the future will be used to indicate a key's validity for different protocols.
        /// </summary>
        public DnsKeyProtocol Protocol { get; private set; }

        /// <summary>
        /// The key algorithm parallel to the same field for the SIG resource.
        /// </summary>
        public DnsAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Optional second 16 bit flag field after the algorithm octet and before the key data.
        /// Must not be non-null unless one or more such additional bits have been defined and are non-zero.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
        public ushort? FlagsExtension { get; private set; }

        /// <summary>
        /// The public key value.
        /// </summary>
        public DataSegment PublicKey { get; private set; }

        /// <summary>
        /// Used in other records to efficiently select between multiple keys which may be applicable and thus check that a public key about to be used for the
        /// computationally expensive effort to check the signature is possibly valid.
        /// </summary>
        public ushort KeyTag
        {
            get
            {
                if (Algorithm == DnsAlgorithm.RsaMd5)
                    return PublicKey.ReadUShort(PublicKey.Length - 3, Endianity.Big);
                ushort flags = BitSequence.Merge((byte)((AuthenticationProhibited ? Mask.AuthenticationProhibited : 0) |
                                                        (ConfidentialityProhibited ? Mask.ConfidentialityProhibited : 0) |
                                                        (Experimental ? Mask.Experimental : 0) | (FlagsExtension.HasValue ? Mask.IsFlagsExtension : 0) |
                                                        (UserAssociated ? Mask.UserAssociated : 0) | (byte)NameType),
                                                 (byte)((IpSec ? Mask.IpSec : 0) | (Email ? Mask.Email : 0) | (byte)Signatory));
                ushort protocolAndAlgorithm = BitSequence.Merge((byte)Protocol, (byte)Algorithm);
                uint sum = (uint)(flags + protocolAndAlgorithm + (FlagsExtension.HasValue ? FlagsExtension.Value : 0) + PublicKey.Sum16Bits());
                return (ushort)(sum + (sum >> 16));
            }
        }

        /// <summary>
        /// Two DnsResourceDataKey are equal iff their authentication prohibited, confidentiality prohibited, experimental, user associated, IPSec, email, 
        /// name type, signatory, protocol, algorithm, flags extension and public key fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataKey other)
        {
            return other != null &&
                   AuthenticationProhibited.Equals(other.AuthenticationProhibited) &&
                   ConfidentialityProhibited.Equals(other.ConfidentialityProhibited) &&
                   Experimental.Equals(other.Experimental) &&
                   UserAssociated.Equals(other.UserAssociated) &&
                   IpSec.Equals(other.IpSec) &&
                   Email.Equals(other.Email) &&
                   NameType.Equals(other.NameType) &&
                   Signatory.Equals(other.Signatory) &&
                   Protocol.Equals(other.Protocol) &&
                   Algorithm.Equals(other.Algorithm) &&
                   (FlagsExtension.HasValue
                        ? other.FlagsExtension.HasValue && FlagsExtension.Value.Equals(other.FlagsExtension.Value)
                        : !other.FlagsExtension.HasValue) &&
                   PublicKey.Equals(other.PublicKey);
        }

        /// <summary>
        /// Two DnsResourceDataKey are equal iff their authentication prohibited, confidentiality prohibited, experimental, user associated, IPSec, email, 
        /// name type, signatory, protocol, algorithm, flags extension and public key fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataKey);
        }

        /// <summary>
        /// A hash code out of the combination of the authentication prohibited, confidentiality prohibited, experimental, user associated, IPSec, email, 
        /// name type, signatory, protocol, algorithm, flags extension and public key fields.
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(
                BitSequence.Merge(BitSequence.Merge(AuthenticationProhibited, ConfidentialityProhibited, Experimental, UserAssociated, IpSec, Email),
                                  (byte)NameType, (byte)Signatory, (byte)Protocol),
                BitSequence.Merge((byte)Algorithm, (ushort)(FlagsExtension.HasValue ? FlagsExtension.Value : 0)),
                PublicKey);

        }

        internal DnsResourceDataKey()
            : this(false, false, false, false, false, false, DnsKeyNameType.ZoneKey, DnsKeySignatoryAttributes.Zone, DnsKeyProtocol.All, DnsAlgorithm.None, null,
                   DataSegment.Empty)
        {
        }

        internal override int GetLength()
        {
            return ConstantPartLength + (FlagsExtension != null ? sizeof(ushort) : 0) + PublicKey.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte flagsByte0 = 0;
            if (AuthenticationProhibited)
                flagsByte0 |= Mask.AuthenticationProhibited;
            if (ConfidentialityProhibited)
                flagsByte0 |= Mask.ConfidentialityProhibited;
            if (Experimental)
                flagsByte0 |= Mask.Experimental;
            if (UserAssociated)
                flagsByte0 |= Mask.UserAssociated;
            if (FlagsExtension.HasValue)
                flagsByte0 |= Mask.IsFlagsExtension;
            flagsByte0 |= (byte)((byte)NameType & Mask.NameType);
            buffer.Write(offset + Offset.AuthenticationProhibited, flagsByte0);

            byte flagsByte1 = 0;
            if (IpSec)
                flagsByte1 |= Mask.IpSec;
            if (Email)
                flagsByte1 |= Mask.Email;
            flagsByte1 |= (byte)((byte)Signatory & Mask.Signatory);
            buffer.Write(offset + Offset.Signatory, flagsByte1);

            buffer.Write(offset + Offset.Protocol, (byte)Protocol);
            buffer.Write(offset + Offset.Algorithm, (byte)Algorithm);

            if (FlagsExtension.HasValue)
                buffer.Write(offset + Offset.FlagsExtension, FlagsExtension.Value, Endianity.Big);

            PublicKey.Write(buffer, offset + Offset.FlagsExtension + (FlagsExtension.HasValue ? sizeof(ushort) : 0));
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < ConstantPartLength)
                return null;

            bool authenticationProhibited = data.ReadBool(Offset.AuthenticationProhibited, Mask.AuthenticationProhibited);
            bool confidentialityProhibited = data.ReadBool(Offset.ConfidentialityProhibited, Mask.ConfidentialityProhibited);
            bool experimental = data.ReadBool(Offset.Experimental, Mask.Experimental);
            bool isFlagsExtension = data.ReadBool(Offset.IsFlagsExtension, Mask.IsFlagsExtension);
            bool userAssociated = data.ReadBool(Offset.UserAssociated, Mask.UserAssociated);
            bool ipSec = data.ReadBool(Offset.IpSec, Mask.IpSec);
            bool email = data.ReadBool(Offset.Email, Mask.Email);
            DnsKeyNameType nameType = (DnsKeyNameType)(data[Offset.NameType] & Mask.NameType);
            DnsKeySignatoryAttributes signatory = (DnsKeySignatoryAttributes)(data[Offset.Signatory] & Mask.Signatory);
            DnsKeyProtocol protocol = (DnsKeyProtocol)data[Offset.Protocol];
            DnsAlgorithm algorithm = (DnsAlgorithm)data[Offset.Algorithm];
            ushort? flagsExtension = (isFlagsExtension ? ((ushort?)data.ReadUShort(Offset.FlagsExtension, Endianity.Big)) : null);
            int publicKeyOffset = Offset.FlagsExtension + (isFlagsExtension ? sizeof(ushort) : 0);
            DataSegment publicKey = data.Subsegment(publicKeyOffset, data.Length - publicKeyOffset);

            return new DnsResourceDataKey(authenticationProhibited, confidentialityProhibited, experimental, userAssociated, ipSec, email, nameType, signatory,
                                          protocol, algorithm, flagsExtension, publicKey);
        }
    }
}