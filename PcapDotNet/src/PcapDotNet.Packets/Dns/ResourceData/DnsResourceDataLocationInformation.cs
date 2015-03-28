using System;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1876.
    /// <pre>
    /// +-----+---------+------+-----------+----------+
    /// | bit | 0-7     | 8-15 | 16-23     | 24-31    |
    /// +-----+---------+------+-----------+----------+
    /// | 0   | VERSION | SIZE | HORIZ PRE | VERT PRE |
    /// +-----+---------+------+-----------+----------+
    /// | 32  | LATITUDE                              |
    /// +-----+---------------------------------------+
    /// | 64  | LONGITUDE                             |
    /// +-----+---------------------------------------+
    /// | 96  | ALTITUDE                              |
    /// +-----+---------------------------------------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Location)]
    public sealed class DnsResourceDataLocationInformation : DnsResourceDataSimple, IEquatable<DnsResourceDataLocationInformation>
    {
        /// <summary>
        /// The maximum value for the size field.
        /// </summary>
        public const ulong MaxSizeValue = 9 * 1000000000L;

        private static class Offset
        {
            public const int Version = 0;
            public const int Size = Version + sizeof(byte);
            public const int HorizontalPrecision = Size + sizeof(byte);
            public const int VerticalPrecision = HorizontalPrecision + sizeof(byte);
            public const int Latitude = VerticalPrecision + sizeof(byte);
            public const int Longitude = Latitude + sizeof(uint);
            public const int Altitude = Longitude + sizeof(uint);
        }

        /// <summary>
        /// The number of bytes this resource data takes.
        /// </summary>
        public const int Length = Offset.Altitude + sizeof(uint);

        /// <summary>
        /// Constructs an instance from the version, size, horizontal precision, vertical precision, latitude, longitude and altitude fields.
        /// </summary>
        /// <param name="version">
        /// Version number of the representation.
        /// This must be zero.
        /// Implementations are required to check this field and make no assumptions about the format of unrecognized versions.
        /// </param>
        /// <param name="size">
        /// The diameter of a sphere enclosing the described entity, in centimeters.
        /// Only numbers of the form decimal digit times 10 in the power of a decimal digit are allowed since it is expressed as a pair of four-bit unsigned integers, 
        /// each ranging from zero to nine, with the most significant four bits representing the base and the second number representing the power of ten by which to multiply the base.
        /// This allows sizes from 0e0 (&lt;1cm) to 9e9(90,000km) to be expressed.
        /// This representation was chosen such that the hexadecimal representation can be read by eye; 0x15 = 1e5.
        /// Four-bit values greater than 9 are undefined, as are values with a base of zero and a non-zero exponent.
        /// 
        /// Since 20000000m (represented by the value 0x29) is greater than the equatorial diameter of the WGS 84 ellipsoid (12756274m),
        /// it is therefore suitable for use as a "worldwide" size.
        /// </param>
        /// <param name="horizontalPrecision">
        /// The horizontal precision of the data, in centimeters, expressed using the same representation as Size.
        /// This is the diameter of the horizontal "circle of error", rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of Size; to get a "plus or minus" value, divide by 2.)
        /// </param>
        /// <param name="verticalPrecision">
        /// The vertical precision of the data, in centimeters, expressed using the sane representation as for Size.
        /// This is the total potential vertical error, rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of SIize; to get a "plus or minus" value, divide by 2.)
        /// Note that if altitude above or below sea level is used as an approximation for altitude relative to the ellipsoid, the precision value should be adjusted.
        /// </param>
        /// <param name="latitude">
        /// The latitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc.
        /// 2^31 represents the equator; numbers above that are north latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc, rounded away from the prime meridian.
        /// 2^31 represents the prime meridian; numbers above that are east longitude.
        /// </param>
        /// <param name="altitude">
        /// The altitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in centimeters,
        /// from a base of 100,000m below the reference spheroid used by GPS (semimajor axis a=6378137.0, reciprocal flattening rf=298.257223563).
        /// Altitude above (or below) sea level may be used as an approximation of altitude relative to the the spheroid,
        /// though due to the Earth's surface not being a perfect spheroid, there will be differences.
        /// (For example, the geoid (which sea level approximates) for the continental US ranges from 10 meters to 50 meters below the spheroid.
        /// Adjustments to Altitude and/or VerticalPrecision will be necessary in most cases.
        /// The Defense Mapping Agency publishes geoid height values relative to the ellipsoid.
        /// </param>
        public DnsResourceDataLocationInformation(byte version, ulong size, ulong horizontalPrecision, ulong verticalPrecision, uint latitude, uint longitude,
                                                  uint altitude)
        {
            if (!IsValidSize(size))
                throw new ArgumentOutOfRangeException("size", size, "Must be in the form <digit> * 10^<digit>.");
            if (!IsValidSize(horizontalPrecision))
                throw new ArgumentOutOfRangeException("horizontalPrecision", horizontalPrecision, "Must be in the form <digit> * 10^<digit>.");
            if (!IsValidSize(verticalPrecision))
                throw new ArgumentOutOfRangeException("verticalPrecision", verticalPrecision, "Must be in the form <digit> * 10^<digit>.");

            Version = version;
            Size = size;
            HorizontalPrecision = horizontalPrecision;
            VerticalPrecision = verticalPrecision;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        /// <summary>
        /// Version number of the representation.
        /// This must be zero.
        /// Implementations are required to check this field and make no assumptions about the format of unrecognized versions.
        /// </summary>
        public byte Version { get; private set; }

        /// <summary>
        /// The diameter of a sphere enclosing the described entity, in centimeters.
        /// Only numbers of the form decimal digit times 10 in the power of a decimal digit are allowed since it is expressed as a pair of four-bit unsigned integers, 
        /// each ranging from zero to nine, with the most significant four bits representing the base and the second number representing the power of ten by which to multiply the base.
        /// This allows sizes from 0e0 (&lt;1cm) to 9e9(90,000km) to be expressed.
        /// This representation was chosen such that the hexadecimal representation can be read by eye; 0x15 = 1e5.
        /// Four-bit values greater than 9 are undefined, as are values with a base of zero and a non-zero exponent.
        /// 
        /// Since 20000000m (represented by the value 0x29) is greater than the equatorial diameter of the WGS 84 ellipsoid (12756274m),
        /// it is therefore suitable for use as a "worldwide" size.
        /// </summary>
        public ulong Size { get; private set; }

        /// <summary>
        /// The horizontal precision of the data, in centimeters, expressed using the same representation as Size.
        /// This is the diameter of the horizontal "circle of error", rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of Size; to get a "plus or minus" value, divide by 2.)
        /// </summary>
        public ulong HorizontalPrecision { get; private set; }

        /// <summary>
        /// The vertical precision of the data, in centimeters, expressed using the sane representation as for Size.
        /// This is the total potential vertical error, rather than a "plus or minus" value.
        /// (This was chosen to match the interpretation of SIize; to get a "plus or minus" value, divide by 2.)
        /// Note that if altitude above or below sea level is used as an approximation for altitude relative to the ellipsoid, the precision value should be adjusted.
        /// </summary>
        public ulong VerticalPrecision { get; private set; }

        /// <summary>
        /// The latitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc.
        /// 2^31 represents the equator; numbers above that are north latitude.
        /// </summary>
        public uint Latitude { get; private set; }

        /// <summary>
        /// The longitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in thousandths of a second of arc, rounded away from the prime meridian.
        /// 2^31 represents the prime meridian; numbers above that are east longitude.
        /// </summary>
        public uint Longitude { get; private set; }

        /// <summary>
        /// The altitude of the center of the sphere described by the Size field, expressed as a 32-bit integer,
        /// most significant octet first (network standard byte order), in centimeters,
        /// from a base of 100,000m below the reference spheroid used by GPS (semimajor axis a=6378137.0, reciprocal flattening rf=298.257223563).
        /// Altitude above (or below) sea level may be used as an approximation of altitude relative to the the spheroid,
        /// though due to the Earth's surface not being a perfect spheroid, there will be differences.
        /// (For example, the geoid (which sea level approximates) for the continental US ranges from 10 meters to 50 meters below the spheroid.
        /// Adjustments to Altitude and/or VerticalPrecision will be necessary in most cases.
        /// The Defense Mapping Agency publishes geoid height values relative to the ellipsoid.
        /// </summary>
        public uint Altitude { get; private set; }

        /// <summary>
        /// Two DnsResourceDataLocationInformation are equal iff their version, size, horizontal precision, vertical precision, latitude, 
        /// longitude and altitude fields are equal.
        /// </summary>
        public bool Equals(DnsResourceDataLocationInformation other)
        {
            return other != null &&
                   Version.Equals(other.Version) &&
                   Size.Equals(other.Size) &&
                   HorizontalPrecision.Equals(other.HorizontalPrecision) &&
                   VerticalPrecision.Equals(other.VerticalPrecision) &&
                   Latitude.Equals(other.Latitude) &&
                   Longitude.Equals(other.Longitude) &&
                   Altitude.Equals(other.Altitude);
        }

        /// <summary>
        /// Two DnsResourceDataLocationInformation are equal iff their version, size, horizontal precision, vertical precision, latitude, 
        /// longitude and altitude fields are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataLocationInformation);
        }

        /// <summary>
        /// A hash code based on the version, size, horizontal precision, vertical precision, latitude, longitude and altitude fields
        /// </summary>
        public override int GetHashCode()
        {
            return Sequence.GetHashCode(Version, Size, HorizontalPrecision, VerticalPrecision, Latitude, Longitude, Altitude);
        }

        internal DnsResourceDataLocationInformation()
            : this(0, 0, 0, 0, 0, 0, 0)
        {
        }

        internal override int GetLength()
        {
            return Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            buffer.Write(offset + Offset.Version, Version);
            WriteSize(buffer, offset + Offset.Size, Size);
            WriteSize(buffer, offset + Offset.HorizontalPrecision, HorizontalPrecision);
            WriteSize(buffer, offset + Offset.VerticalPrecision, VerticalPrecision);
            buffer.Write(offset + Offset.Latitude, Latitude, Endianity.Big);
            buffer.Write(offset + Offset.Longitude, Longitude, Endianity.Big);
            buffer.Write(offset + Offset.Altitude, Altitude, Endianity.Big);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length != Length)
                return null;

            byte version = data[Offset.Version];
            ulong size = ReadSize(data[Offset.Size]);
            if (size > MaxSizeValue)
                return null;
            ulong horizontalPrecision = ReadSize(data[Offset.HorizontalPrecision]);
            if (horizontalPrecision > MaxSizeValue)
                return null;
            ulong verticalPrecision = ReadSize(data[Offset.VerticalPrecision]);
            if (verticalPrecision > MaxSizeValue)
                return null;
            uint latitude = data.ReadUInt(Offset.Latitude, Endianity.Big);
            uint longitude = data.ReadUInt(Offset.Longitude, Endianity.Big);
            uint altitude = data.ReadUInt(Offset.Altitude, Endianity.Big);

            return new DnsResourceDataLocationInformation(version, size, horizontalPrecision, verticalPrecision, latitude, longitude, altitude);
        }

        private static bool IsValidSize(ulong size)
        {
            if (size == 0)
                return true;
            if (size > MaxSizeValue)
                return false;
            while (size % 10 == 0)
                size /= 10;

            return size <= 9;
        }

        private static void WriteSize(byte[] buffer, int offset, ulong size)
        {
            byte baseValue;
            byte exponent;
            if (size == 0)
            {
                baseValue = 0;
                exponent = 0;
            }
            else
            {
                exponent = (byte)Math.Log10(size);
                baseValue = (byte)(size / Math.Pow(10, exponent));
            }

            byte value = (byte)((baseValue << 4) | exponent);
            buffer.Write(offset, value);
        }

        private static ulong ReadSize(byte value)
        {
            return (ulong)((value >> 4) * Math.Pow(10, value & 0x0F));
        }
    }
}