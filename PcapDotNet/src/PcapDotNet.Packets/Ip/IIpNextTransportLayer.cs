namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// A Transport layer under an IP layer.
    /// Must supply information about the Transport layer checksum.
    /// </summary>
    public interface IIpNextTransportLayer : IIpNextLayer
    {
        /// <summary>
        /// Checksum is the 16-bit one's complement of the one's complement sum of a pseudo header of information from the IP header, 
        /// the Transport header, and the data, padded with zero octets at the end (if necessary) to make a multiple of two octets.
        /// If null is given, the Checksum will be calculated to be correct according to the data.
        /// </summary>
        ushort? Checksum { get; set; }

        /// <summary>
        /// Whether the checksum should be calculated.
        /// Can be false in UDP because the checksum is optional. false means that the checksum will be left zero.
        /// </summary>
        bool CalculateChecksum { get; }

        /// <summary>
        /// The offset in the layer where the checksum should be written.
        /// </summary>
        int ChecksumOffset { get; }

        /// <summary>
        /// Whether the checksum is optional in the layer.
        /// </summary>
        bool IsChecksumOptional { get; }
    }
}