namespace PcapDotNet.Packets
{
    /// <summary>
    /// A factory interface for an unknown option.
    /// </summary>
    /// <typeparam name="TOptionType">The option type enum type.</typeparam>
    public interface IOptionUnknownFactory<in TOptionType>
    {
        /// <summary>
        /// Creates an unknown option from its type and by reading a buffer for its value.
        /// </summary>
        /// <param name="optionType">The type of the unknown option.</param>
        /// <param name="buffer">The buffer of bytes to read the value of the unknown option.</param>
        /// <param name="offset">The offset in the buffer to start reading the bytes.</param>
        /// <param name="valueLength">The number of bytes to read from the buffer.</param>
        /// <returns>An option created from the given type and buffer.</returns>
        Option CreateInstance(TOptionType optionType, byte[] buffer, ref int offset, byte valueLength);
    }
}