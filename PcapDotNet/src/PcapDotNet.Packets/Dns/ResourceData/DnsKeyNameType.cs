namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2535.
    /// </summary>
    public enum DnsKeyNameType : byte
    {
        /// <summary>
        /// Indicates that this is a key associated with a "user" or "account" at an end entity, usually a host.
        /// The coding of the owner name is that used for the responsible individual mailbox in the SOA and RP RRs:
        /// The owner name is the user name as the name of a node under the entity name.
        /// For example, "j_random_user" on host.subdomain.example could have a public key associated through a KEY RR with name j_random_user.host.subdomain.example.
        /// It could be used in a security protocol where authentication of a user was desired.
        /// This key might be useful in IP or other security for a user level service such a telnet, ftp, rlogin, etc.
        /// </summary>
        UserOrAccountAtEndEntity = 0,

        /// <summary>
        /// Indicates that this is a zone key for the zone whose name is the KEY RR owner name.
        /// This is the public key used for the primary DNS security feature of data origin authentication.
        /// Zone KEY RRs occur only at delegation points.
        /// </summary>
        ZoneKey = 1,

        /// <summary>
        /// Indicates that this is a key associated with the non-zone "entity" whose name is the RR owner name.
        /// This will commonly be a host but could, in some parts of the DNS tree, be some other type of entity such as a telephone number [RFC 1530] or numeric IP address.
        /// This is the public key used in connection with DNS request and transaction authentication services.
        /// It could also be used in an IP-security protocol where authentication at the host, rather than user, level was desired, such as routing, NTP, etc.
        /// </summary>
        NonZoneEntity = 2,
    }
}