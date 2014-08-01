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

        public const int OptionDataLength = Offset.LongitudeDegrees + UInt24.SizeOf;

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
            get { return ToReal(LatitudeDegrees); }
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
            bool isPositive = twosComplementFixedPointWith9WholeBits >> 23 == 1;
            int integerPart = (twosComplementFixedPointWith9WholeBits & 0x7F8000) >> 15;
            int fractionPart = twosComplementFixedPointWith9WholeBits & 0x007FFF;
            return (isPositive ? 1 : -1) *
                   (integerPart + (((double)fractionPart) / (1 << 15)));
        }

        private bool EqualsData(IpV6AccessNetworkIdentifierSubOptionGeoLocation other)
        {
            return other != null &&
                   LatitudeDegrees == other.LatitudeDegrees && LongitudeDegrees == other.LongitudeDegrees;
        }
    }
}