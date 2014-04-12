namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// </summary>
    public enum IpV6AccessNetworkIdentifierOperatorIdentifierType : byte
    {
        /// <summary>
        /// Operator-Identifier as a variable-length Private Enterprise Number (PEN) encoded in a network-byte order.
        /// The maximum PEN value depends on the ANI Length and is calculated using the formula: maximum PEN = 2^((ANI_length-1)*8)-1.
        /// For example, the ANI Length of 4 allows for encoding PENs from 0 to 2^24-1, i.e., from 0 to 16777215,
        /// and uses 3 octets of Operator-Identifier space.
        /// </summary>
        PrivateEnterpriseNumber = 1,

        /// <summary>
        /// Realm of the operator.
        /// Realm names are required to be unique and are piggybacked on the administration of the DNS namespace.
        /// Realms meet the syntactic requirements of the "Preferred Name Syntax".
        /// They are encoded as US-ASCII.
        /// 3GPP specifications also define realm names that can be used to convey PLMN Identifiers.
        /// </summary>
        RealmOfTheOperator = 2,
    }
}