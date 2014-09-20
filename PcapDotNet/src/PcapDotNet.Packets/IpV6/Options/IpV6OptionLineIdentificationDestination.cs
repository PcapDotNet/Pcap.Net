using System;
using System.Globalization;

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
    public sealed class IpV6OptionLineIdentificationDestination : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        private static class Offset
        {
            public const int LineIdentificationLength = 0;
            public const int LineIdentification = LineIdentificationLength + sizeof(byte);
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.LineIdentification;

        /// <summary>
        /// Creates an instance from line identification.
        /// </summary>
        /// <param name="lineIdentification">
        /// Variable length data inserted by the AN describing the subscriber agent circuit identifier 
        /// corresponding to the logical access loop port of the AN from which the RS was initiated. 
        /// The line identification must be unique across all the ANs that share a link to the edge router.
        /// </param>
        public IpV6OptionLineIdentificationDestination(DataSegment lineIdentification)
            : base(IpV6OptionType.LineIdentification)
        {
            if (lineIdentification.Length > byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("lineIdentification", lineIdentification,
                                                      string.Format(CultureInfo.InvariantCulture, "Cannot be longer than {0} bytes.", byte.MaxValue));
            }
            LineIdentification = lineIdentification;
        }

        /// <summary>
        /// Variable length data inserted by the AN describing the subscriber agent circuit identifier 
        /// corresponding to the logical access loop port of the AN from which the RS was initiated. 
        /// The line identification must be unique across all the ANs that share a link to the edge router.
        /// </summary>
        public DataSegment LineIdentification { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
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

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionLineIdentificationDestination);
        }

        internal override int GetDataHashCode()
        {
            return LineIdentification.GetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, (byte)LineIdentification.Length);
            buffer.Write(ref offset, LineIdentification);
        }

        private IpV6OptionLineIdentificationDestination()
            : this(DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6OptionLineIdentificationDestination other)
        {
            return other != null &&
                   LineIdentification.Equals(other.LineIdentification);
        }
    }
}