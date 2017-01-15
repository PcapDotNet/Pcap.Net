using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    /// <summary>
    /// RFC 2132.
    /// This option specifies a list of static routes that the client should
    /// install in its routing cache.If multiple routes to the same
    /// destination are specified, they are listed in descending order of
    /// priority.
    /// <pre>
    ///  Code   Len         Destination 1           Router 1
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
    /// |  33 |  n  |  d1 |  d2 |  d3 |  d4 |  r1 |  r2 |  r3 |  r4 |
    /// +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+
    ///         Destination 2           Router 2
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// |  d1 |  d2 |  d3 |  d4 |  r1 |  r2 |  r3 |  r4 | ...
    /// +-----+-----+-----+-----+-----+-----+-----+-----+---
    /// </pre>
    /// </summary>
    public class DhcpStaticRouteOption : DhcpOption
    {
        internal const int MAX_ROUTES = 255 / 8;

        /// <summary>
        /// create new DhcpStaticRouteOption.
        /// </summary>
        /// <param name="routes">Routes</param>
        public DhcpStaticRouteOption(IList<IpV4AddressRoute> routes) : base(DhcpOptionCode.StaticRoute)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (routes.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(routes), routes.Count, "The minimum items in routes is 1");
            if (routes.Count > MAX_ROUTES)
                throw new ArgumentOutOfRangeException(nameof(routes), routes.Count, "The maximum items in routes is " + MAX_ROUTES);

            Routes = new ReadOnlyCollection<IpV4AddressRoute>(routes);
        }

        internal static DhcpStaticRouteOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            if (length % 8 != 0)
                throw new ArgumentException("length has to be a multiple of 8");
            IList<IpV4Address> addresses = DhcpAddressListOption.GetAddresses(data, length, ref offset);

            List<IpV4AddressRoute> routes = new List<IpV4AddressRoute>();
            for (int i = 0; i < addresses.Count; i += 2)
            {
                routes.Add(new IpV4AddressRoute(addresses[i], addresses[i + 1]));
            }
            return new DhcpStaticRouteOption(routes);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            foreach (IpV4AddressRoute route in Routes)
            {
                buffer.Write(ref offset, route.Destination, Endianity.Big);
                buffer.Write(ref offset, route.Router, Endianity.Big);
            }
        }

        /// <summary>
        /// Length of the Dhcp-Option.
        /// </summary>
        public override byte Length
        {
            get
            {
                return (byte)(Routes.Count * 8);
            }
        }

        /// <summary>
        /// Routes.
        /// </summary>
        public IReadOnlyCollection<IpV4AddressRoute> Routes
        {
            get;
            private set;
        }

        /// <summary>
        /// Mapping between Ipv$Address and Route Address.
        /// </summary>
        public struct IpV4AddressRoute : IEquatable<IpV4AddressRoute>
        {
            /// <summary>
            /// The number of bytes the IpV4AddressRoute take.
            /// </summary>
            public const int SizeOf = IpV4Address.SizeOf + IpV4Address.SizeOf;

            /// <summary>
            /// Destination Address.
            /// </summary>
            public IpV4Address Destination

            {
                get;
                private set;
            }

            /// <summary>
            /// Router-IP.
            /// </summary>
            public IpV4Address Router
            {
                get;
                private set;
            }

            /// <summary>
            /// create new IpV4AddressRoute.
            /// </summary>
            /// <param name="destination">Destination IP</param>
            /// <param name="router">Router IP</param>
            public IpV4AddressRoute(IpV4Address destination, IpV4Address router)
            {
                Destination = destination;
                Router = router;
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is IpV4AddressRoute)
                {
                    return Equals((IpV4AddressRoute)obj);
                }
                return false;
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="other">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public bool Equals(IpV4AddressRoute other)
            {
                return object.Equals(Destination, other.Destination) &&
                    object.Equals(Router, other.Router);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code.</returns>
            public override int GetHashCode()
            {
                return Destination.GetHashCode() ^ Router.GetHashCode();
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="left">left IpV4AddressWithMask</param>
            /// <param name="right">right IpV4AddressWithMask</param>
            /// <returns></returns>
            public static bool operator ==(IpV4AddressRoute left, IpV4AddressRoute right)
            {
                return Object.Equals(left, right);
            }

            /// <summary>
            /// Determines whether the IpV4AddressWithMask is not equal to the current IpV4AddressWithMask.
            /// </summary>
            /// <param name="left">left IpV4AddressWithMask</param>
            /// <param name="right">right IpV4AddressWithMask</param>
            /// <returns></returns>
            public static bool operator !=(IpV4AddressRoute left, IpV4AddressRoute right)
            {
                return !Object.Equals(left, right);
            }
        }
    }
}