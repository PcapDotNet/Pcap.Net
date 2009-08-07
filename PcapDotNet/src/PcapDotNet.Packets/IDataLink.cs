namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents a datalink.
    /// </summary>
    public interface IDataLink
    {
        /// <summary>
        /// The kind of the datalink.
        /// </summary>
        DataLinkKind Kind { get; }
    }
}