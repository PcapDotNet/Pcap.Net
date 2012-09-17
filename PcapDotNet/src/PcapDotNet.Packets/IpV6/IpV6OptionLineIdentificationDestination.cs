using System;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC ietf-6man-lineid-08.
    /// http://tools.ietf.org/html/draft-ietf-6man-lineid-08
    /// Line Identification Destination Option.
    /// <pre>
    /// +-----+---------------------+
    /// | Bit | 0-7                 |
    /// +-----+---------------------+
    /// | 0   | Option Type         |
    /// +-----+---------------------+
    /// | 8   | Opt Data Len        |
    /// +-----+---------------------+
    /// | 16  | LineIDLen           |
    /// +-----+---------------------+
    /// | 24  | Line Identification |
    /// | ... |                     |
    /// +-----+---------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.LineIdentification)]
    public class IpV6OptionLineIdentificationDestination : IpV6OptionComplex
    {
        private static class Offset
        {
            public const int LineIdentificationLength = 0;
            public const int LineIdentification = LineIdentificationLength + sizeof(byte);
        }

        public const int OptionDataMinimumLength = Offset.LineIdentification;

        public IpV6OptionLineIdentificationDestination(DataSegment lineIdentification)
            : base(IpV6OptionType.LineIdentification)
        {
            if (lineIdentification.Length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("lineIdentification", lineIdentification,
                                                      string.Format("Cannot be longer than {0} bytes.", byte.MaxValue));
            }
            LineIdentification = lineIdentification;
        }

        /// <summary>
        /// Variable length data inserted by the AN describing the subscriber agent circuit identifier 
        /// corresponding to the logical access loop port of the AN from which the RS was initiated. 
        /// The line identification must be unique across all the ANs that share a link to the edge router.
        /// </summary>
        public DataSegment LineIdentification { get; private set; }

        internal IpV6OptionLineIdentificationDestination()
            : this(DataSegment.Empty)
        {
        }

        internal override IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            int lineIdentificationLegnth = data[Offset.LineIdentificationLength];
            if (data.Length != OptionDataMinimumLength + lineIdentificationLegnth)
                return null;

            DataSegment lineIdentification = data.Subsegment(Offset.LineIdentification, data.Length - Offset.LineIdentification);
            return new IpV6OptionLineIdentificationDestination(lineIdentification);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + LineIdentification.Length; }
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)LineIdentification.Length);
            buffer.Write(ref offset, LineIdentification);
        }
    }
}