namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// This interface is used to create all complex options.
    /// Every complex option should implement such a factory to create itself from a buffer.
    /// </summary>
//    internal interface IIpv4OptionComplexFactory
//    {
//        /// <summary>
//        /// Tries to read the option from a buffer starting from the option value (after the type and length).
//        /// </summary>
//        /// <param name="buffer">The buffer to read the option from.</param>
//        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
//        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
//        /// <returns>On success - the complex option read. On failure - null.</returns>
//        IpV4OptionComplex CreateInstance(byte[] buffer, ref int offset, byte valueLength);
//    }
}