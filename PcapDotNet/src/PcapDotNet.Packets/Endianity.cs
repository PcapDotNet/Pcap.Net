namespace PcapDotNet.Packets
{
    /// <summary>
    /// The two possible endianities.
    /// </summary>
    public enum Endianity : byte
    {
        /// <summary>
        /// Small endianity - bytes are read from the high offset to the low offset.
        /// </summary>
        Small,

        /// <summary>
        /// Big endianity - bytes are read from the low offset to the high offset.
        /// </summary>
        Big
    }
}