using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// <pre>
    /// +-----+-------------------+
    /// | Bit | 0-7               |
    /// +-----+-------------------+
    /// | 0   | ANI Type          |
    /// +-----+-------------------+
    /// | 8   | ANI Length        |
    /// +-----+-------------------+
    /// | 16  | Latitude Degrees  | 
    /// |     |                   | 
    /// |     |                   | 
    /// +-----+-------------------+
    /// | 40  | Longitude Degrees |
    /// |     |                   | 
    /// |     |                   | 
    /// +-----+-------------------+
    /// </pre>
    /// </summary>
    [IpV6AccessNetworkIdentifierSubOptionTypeRegistration(IpV6AccessNetworkIdentifierSubOptionType.GeoLocation)]
    public sealed class IpV6AccessNetworkIdentifierSubOptionGeoLocation : IpV6AccessNetworkIdentifierSubOption
    {
        private static class Offset
        {
            public const int LatitudeDegrees = 0;
            public const int LongitudeDegrees = LatitudeDegrees + UInt24.SizeOf;
        }

        /// <summary>
        /// The number of bytes this option data takes.
        /// </summary>
        public const int OptionDataLength = Offset.LongitudeDegrees + UInt24.SizeOf;

        /// <summary>
        /// Creates an instance from latitude and longtitude degrees using integer numbers encoded as a two's complement, fixed point number with 9 whole bits.
        /// See <see cref="IpV6AccessNetworkIdentifierSubOptionGeoLocation.CreateFromRealValues"/> for using real numbers for the degrees.
        /// </summary>
        /// <param name="latitudeDegrees">
        /// A 24-bit latitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </param>
        /// <param name="longitudeDegrees">
        /// A 24-bit longitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// The value ranges from -180 to +180 degrees.
        /// </param>
        public IpV6AccessNetworkIdentifierSubOptionGeoLocation(UInt24 latitudeDegrees, UInt24 longitudeDegrees)
            : base(IpV6AccessNetworkIdentifierSubOptionType.GeoLocation)
        {
            LatitudeDegrees = latitudeDegrees;
            LongitudeDegrees = longitudeDegrees;

            double latitudeDegreesReal = LatitudeDegreesReal;
            if (latitudeDegreesReal < -90 || latitudeDegreesReal > 90)
                throw new ArgumentOutOfRangeException("latitudeDegrees", latitudeDegrees, string.Format("LatitudeDegreesReal is {0} and must be in [-90, 90] range.", latitudeDegreesReal));

            double longtitudeDegreesReal = LongitudeDegreesReal;
            if (longtitudeDegreesReal < -180 || longtitudeDegreesReal > 180)
                throw new ArgumentOutOfRangeException("longitudeDegrees", longitudeDegrees, string.Format("LongitudeDegreesReal is {0} and must be in [-180, 180] range.", longtitudeDegreesReal));
        }

        /// <summary>
        /// Creates an instance from latitude and longtitude degrees using real numbers.
        /// </summary>
        /// <param name="latitudeDegreesReal">
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </param>
        /// <param name="longtitudeDegreesReal">
        /// The value ranges from -180 to +180 degrees.
        /// </param>
        /// <returns>An instance created from the given real degrees values.</returns>
        public static IpV6AccessNetworkIdentifierSubOptionGeoLocation CreateFromRealValues(double latitudeDegreesReal, double longtitudeDegreesReal)
        {
            if (latitudeDegreesReal < -90 || latitudeDegreesReal > 90)
                throw new ArgumentOutOfRangeException("latitudeDegreesReal", latitudeDegreesReal, string.Format("LatitudeDegreesReal is {0} and must be in [-90, 90] range.", latitudeDegreesReal));

            if (longtitudeDegreesReal < -180 || longtitudeDegreesReal > 180)
                throw new ArgumentOutOfRangeException("longtitudeDegreesReal", longtitudeDegreesReal, string.Format("LongitudeDegreesReal is {0} and must be in [-180, 180] range.", longtitudeDegreesReal));

            return new IpV6AccessNetworkIdentifierSubOptionGeoLocation(ToInteger(latitudeDegreesReal), ToInteger(longtitudeDegreesReal));
        }

        /// <summary>
        /// A 24-bit latitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </summary>
        public UInt24 LatitudeDegrees { get; private set; }

        /// <summary>
        /// Positive degrees correspond to the Northern Hemisphere and negative degrees correspond to the Southern Hemisphere.
        /// The value ranges from -90 to +90 degrees.
        /// </summary>
        public double LatitudeDegreesReal
        {
            get { return ToReal(LatitudeDegrees); }
        }

        /// <summary>
        /// A 24-bit longitude degree value encoded as a two's complement, fixed point number with 9 whole bits.
        /// The value ranges from -180 to +180 degrees.
        /// </summary>
        public UInt24 LongitudeDegrees { get; private set; }

        /// <summary>
        /// The value ranges from -180 to +180 degrees.
        /// </summary>
        public double LongitudeDegreesReal
        {
            get { return ToReal(LongitudeDegrees); }
        }

        internal override IpV6AccessNetworkIdentifierSubOption CreateInstance(DataSegment data)
        {
            if (data.Length != OptionDataLength)
                return null;

            UInt24 latitudeDegrees = data.ReadUInt24(Offset.LatitudeDegrees, Endianity.Big);
            UInt24 longitudeDegrees = data.ReadUInt24(Offset.LongitudeDegrees, Endianity.Big);

            return new IpV6AccessNetworkIdentifierSubOptionGeoLocation(latitudeDegrees, longitudeDegrees);
        }

        internal override int DataLength
        {
            get { return OptionDataLength; }
        }

        internal override bool EqualsData(IpV6AccessNetworkIdentifierSubOption other)
        {
            return EqualsData(other as IpV6AccessNetworkIdentifierSubOptionGeoLocation);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(LongitudeDegrees, LatitudeDegrees);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            buffer.Write(offset + Offset.LatitudeDegrees, LatitudeDegrees, Endianity.Big);
            buffer.Write(offset + Offset.LongitudeDegrees, LongitudeDegrees, Endianity.Big);
            offset += OptionDataLength;
        }

        private IpV6AccessNetworkIdentifierSubOptionGeoLocation()
            : this(0, 0)
        {
        }

        private static double ToReal(UInt24 twosComplementFixedPointWith9WholeBits)
        {
            return ((double)(-(twosComplementFixedPointWith9WholeBits >> 23) * (1 << 23)) + (twosComplementFixedPointWith9WholeBits & 0x7FFFFF)) / (1 << 15);
        }

        private static UInt24 ToInteger(double realValue)
        {
            if (realValue >= 0)
                return (UInt24)((int)(realValue * (1 << 15)));
            return (UInt24)((~ToInteger(-realValue)) + 1);
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionGeoLocation other)
        {
            return other != null &&
                   LatitudeDegrees == other.LatitudeDegrees && LongitudeDegrees == other.LongitudeDegrees;
        }
    }
}