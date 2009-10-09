namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// <pre>
    /// +-----+---------+---------------+
    /// | Bit | 0-7     | 8-31          |
    /// +-----+---------+---------------+
    /// | 0   | Pointer | unused        |
    /// +-----+---------+---------------+
    /// | 32  | Internet Header         |
    /// |     | + 64 bits of            |
    /// |     | Original Data Datagram  |
    /// +-----+-------------------------+
    /// </pre>
    /// </summary>
    public class IcmpParameterProblemDatagram : IcmpIpV4PayloadDatagram
    {
        private class Offset
        {
            public const int Pointer = 0;
        }

        /// <summary>
        /// The pointer identifies the octet of the original datagram's header where the error was detected (it may be in the middle of an option).  
        /// For example, 1 indicates something is wrong with the Type of Service, and (if there are options present) 20 indicates something is wrong with the type code of the first option.
        /// </summary>
        public byte Pointer
        {
            get { return this[Offset.Pointer]; }
        }

        internal IcmpParameterProblemDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }
    }
}