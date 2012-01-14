using System;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2137.
    /// </summary>
    [Flags]
    public enum DnsKeySignatory : byte
    {
        /// <summary>
        /// The general update signatory field bit has no special meaning.
        /// If the other three bits are all zero, it must be one so that the field is non-zero to designate that the key is an update key.
        /// The meaning of all values of the signatory field with the general bit and one or more other signatory field bits on is reserved.
        /// </summary>
        General = 1,

        /// <summary>
        /// If non-zero, this key is authorized to add and update RRs for only a single owner name.
        /// If there already exist RRs with one or more names signed by this key, they may be updated but no new name created until the number of existing names is reduced to zero.
        /// This bit is meaningful only for mode A dynamic zones and is ignored in mode B dynamic zones. 
        /// This bit is meaningful only if the owner name is a wildcard.
        /// (Any dynamic update KEY with a non-wildcard name is, in effect, a unique name update key.)
        /// </summary>
        Unique = 2,

        /// <summary>
        /// If non-zero, this key is authorized to add and delete RRs even if there are other RRs with the same owner name and class that are authenticated by a SIG signed with a different dynamic update KEY.
        /// If zero, the key can only authorize updates where any existing RRs of the same owner and class are authenticated by a SIG using the same key.
        /// This bit is meaningful only for type A dynamic zones and is ignored in type B dynamic zones.
        /// 
        /// Keeping this bit zero on multiple KEY RRs with the same or nested wild card owner names permits multiple entities to exist that can create and delete names but can not effect RRs with different owner names from any they created.
        /// In effect, this creates two levels of dynamic update key, strong and weak, where weak keys are limited in interfering with each other but a strong key can interfere with any weak keys or other strong keys.
        /// </summary>
        Strong = 4,

        /// <summary>
        /// If nonzero, this key is authorized to attach, detach, and move zones by creating and deleting NS, glue A, and zone KEY RR(s).
        /// If zero, the key can not authorize any update that would effect such RRs.
        /// This bit is meaningful for both type A and type B dynamic secure zones.
        /// NOTE:  do not confuse the "zone" signatory field bit with the "zone" key type bit.
        /// </summary>
        Zone = 8,
    }
}