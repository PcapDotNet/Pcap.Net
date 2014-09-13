namespace PcapDotNet.Packets.IpV6
{
    internal interface IIpV6OptionComplexFactory
    {
        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        IpV6Option CreateInstance(DataSegment data);
    }
}