using System;
using System.Text;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1712.
    /// <pre>
    /// +-----------+
    /// | LONGITUDE |
    /// +-----------+
    /// | LATITUDE  |
    /// +-----------+
    /// | ALTITUDE  |
    /// +-----------+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.GPos)]
    public sealed class DnsResourceDataGeographicalPosition : DnsResourceDataSimple, IEquatable<DnsResourceDataGeographicalPosition>
    {
        public DnsResourceDataGeographicalPosition(string longitude, string latitude, string altitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
        }

        /// <summary>
        /// The real number describing the longitude encoded as a printable string.
        /// The precision is limited by 256 charcters within the range -90..90 degrees.
        /// Positive numbers indicate locations north of the equator.
        /// </summary>
        public string Longitude { get; private set; }

        /// <summary>
        /// The real number describing the latitude encoded as a printable string.
        /// The precision is limited by 256 charcters within the range -180..180 degrees.
        /// Positive numbers indicate locations east of the prime meridian.
        /// </summary>
        public string Latitude { get; private set; }

        /// <summary>
        /// The real number describing the altitude (in meters) from mean sea-level encoded as a printable string.
        /// The precision is limited by 256 charcters.
        /// Positive numbers indicate locations above mean sea-level.
        /// </summary>
        public string Altitude { get; private set; }

        public bool Equals(DnsResourceDataGeographicalPosition other)
        {
            return other != null &&
                   Longitude.Equals(other.Longitude) &&
                   Latitude.Equals(other.Latitude) &&
                   Altitude.Equals(other.Altitude);

        }

        public override bool Equals(DnsResourceData other)
        {
            return Equals(other as DnsResourceDataGeographicalPosition);
        }

        internal DnsResourceDataGeographicalPosition()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        internal override int GetLength()
        {
            return Encoding.ASCII.GetByteCount(Longitude) + 1 +
                   Encoding.ASCII.GetByteCount(Latitude) + 1 +
                   Encoding.ASCII.GetByteCount(Altitude) + 1;
        }


        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            byte[] longtitudeBytes = Encoding.ASCII.GetBytes(Longitude);
            byte[] latitudeBytes = Encoding.ASCII.GetBytes(Latitude);
            byte[] altitudeBytes = Encoding.ASCII.GetBytes(Altitude);

            buffer.Write(ref offset, (byte)longtitudeBytes.Length);
            buffer.Write(ref offset, longtitudeBytes);
            buffer.Write(ref offset, (byte)latitudeBytes.Length);
            buffer.Write(ref offset, latitudeBytes);
            buffer.Write(ref offset, (byte)altitudeBytes.Length);
            buffer.Write(ref offset, altitudeBytes);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            if (data.Length < 3)
                return null;

            int longtitudeNumBytes = data[0];
            if (data.Length < longtitudeNumBytes + 3)
                return null;
            string longtitude = data.SubSegment(1, longtitudeNumBytes).ToString(Encoding.ASCII);
            data = data.SubSegment(longtitudeNumBytes + 1, data.Length - longtitudeNumBytes - 1);

            int latitudeNumBytes = data[0];
            if (data.Length < latitudeNumBytes + 2)
                return null;
            string latitude = data.SubSegment(1, latitudeNumBytes).ToString(Encoding.ASCII);
            data = data.SubSegment(latitudeNumBytes + 1, data.Length - latitudeNumBytes - 1);

            int altitudeNumBytes = data[0];
            if (data.Length != altitudeNumBytes + 1)
                return null;
            string altitude = data.SubSegment(1, altitudeNumBytes).ToString(Encoding.ASCII);

            return new DnsResourceDataGeographicalPosition(longtitude, latitude, altitude);
        }
    }
}