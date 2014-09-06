using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Charles Lynn.
    /// http://ana-3.lcs.mit.edu/~jnc/nimrod/eidoption.txt
    /// Endpoint Identifier Option.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Src Len     | Dst Len      |
    /// +-----+-------------+--------------+
    /// | 32  | Source EID                 |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// |     | Destination EID            |
    /// | ... |                            |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.EndpointIdentification)]
    public sealed class IpV6OptionEndpointIdentification : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        private static class Offset
        {
            public const int SourceEndpointIdentifierLength = 0;
            public const int DestinationEndpointIdentifierLength = SourceEndpointIdentifierLength + sizeof(byte);
            public const int SourceEndpointIdentifier = DestinationEndpointIdentifierLength + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.SourceEndpointIdentifier;

        public IpV6OptionEndpointIdentification(DataSegment sourceEndpointIdentifier, DataSegment destinationEndpointIdentifier)
            : base(IpV6OptionType.EndpointIdentification)
        {
            SourceEndpointIdentifier = sourceEndpointIdentifier;
            DestinationEndpointIdentifier = destinationEndpointIdentifier;
        }

        public DataSegment SourceEndpointIdentifier { get; private set; }

        public DataSegment DestinationEndpointIdentifier { get; private set; }

        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;
            
            int sourceEndpointIdentifierLength = data[Offset.SourceEndpointIdentifierLength];
            int destinationEndpointIdentifierLength = data[Offset.DestinationEndpointIdentifierLength];
            if (data.Length != OptionDataMinimumLength + sourceEndpointIdentifierLength + destinationEndpointIdentifierLength)
                return null;

            DataSegment sourceEndpointIdentifier = data.Subsegment(Offset.SourceEndpointIdentifier, sourceEndpointIdentifierLength);
            int destinationEndpointIdentifierOffset = Offset.SourceEndpointIdentifier + sourceEndpointIdentifierLength;
            DataSegment destinationEndpointIdentifier = data.Subsegment(destinationEndpointIdentifierOffset, destinationEndpointIdentifierLength);
            return new IpV6OptionEndpointIdentification(sourceEndpointIdentifier, destinationEndpointIdentifier);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + SourceEndpointIdentifier.Length + DestinationEndpointIdentifier.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionEndpointIdentification);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(SourceEndpointIdentifier, DestinationEndpointIdentifier);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)SourceEndpointIdentifier.Length);
            buffer.Write(ref offset, (byte)DestinationEndpointIdentifier.Length);
            buffer.Write(ref offset, SourceEndpointIdentifier);
            buffer.Write(ref offset, DestinationEndpointIdentifier);
        }

        private IpV6OptionEndpointIdentification()
            : this(DataSegment.Empty, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6OptionEndpointIdentification other)
        {
            return other != null &&
                   SourceEndpointIdentifier.Equals(other.SourceEndpointIdentifier) && DestinationEndpointIdentifier.Equals(other.DestinationEndpointIdentifier);
        }
    }
}