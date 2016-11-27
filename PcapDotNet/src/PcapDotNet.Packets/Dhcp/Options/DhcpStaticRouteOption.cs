using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Dhcp.Options
{
    public class DhcpStaticRouteOption : DhcpOption
    {
        internal const int MAX_ROUTES = 255 / 8;

        public DhcpStaticRouteOption(IList<IpV4AddressRoute> routes) : base(DhcpOptionCode.StaticRoute)
        {
            if (routes == null)
                throw new ArgumentNullException(nameof(routes));
            if (routes.Count > MAX_ROUTES)
                throw new ArgumentOutOfRangeException(nameof(routes), routes.Count, $"The maximum items in routes is {MAX_ROUTES}");

            Routes = new ReadOnlyCollection<IpV4AddressRoute>(routes);
        }

        internal static DhcpStaticRouteOption Read(DataSegment data, ref int offset)
        {
            byte length = data[offset++];
            if (length % 8 != 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "length has to be a multiple of 8");
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

        public override byte Length
        {
            get
            {
                if (Routes.Count > MAX_ROUTES)
                    throw new ArgumentOutOfRangeException(nameof(Routes), Routes.Count, $"The maximum items in Routes is {MAX_ROUTES}");

                return (byte)(Routes.Count * 8);
            }
        }

        public IReadOnlyCollection<IpV4AddressRoute> Routes
        {
            get;
            private set;
        }

        public struct IpV4AddressRoute
        {
            public IpV4Address Destination

            {
                get;
                private set;
            }

            public IpV4Address Router
            {
                get;
                private set;
            }

            public IpV4AddressRoute(IpV4Address destination, IpV4Address router)
            {
                Destination = destination;
                Router = router;
            }
        }
    }
}