using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Icmp
{
    internal static class IcmpDatagramFactory
    {
        internal static IcmpDatagram CreateInstance(IcmpMessageType messageType, byte[] buffer, int offset, int length)
        {
            IcmpDatagram prototype;
            if (!_prototypes.TryGetValue(messageType, out prototype))
                return new IcmpUnknownDatagram(buffer, offset, length);

            return prototype.CreateInstance(buffer, offset, length);
        }

        private static Dictionary<IcmpMessageType, IcmpDatagram> InitializeComplexOptions()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IcmpDatagram).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                let constructor =
                    type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new[] {typeof(byte[]), typeof(int), typeof(int)}, null)
                select new
                       {
                           GetRegistrationAttribute(type).MessageType,
                           Datagram = (IcmpDatagram)constructor.Invoke(new object[] {new byte[0], 0, 0})
                       };

            return prototypes.ToDictionary(prototype => prototype.MessageType, prototype => prototype.Datagram);
        }

        private static IcmpDatagramRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registrationAttributes =
                from attribute in type.GetCustomAttributes<IcmpDatagramRegistrationAttribute>(false)
                select attribute;

            if (!registrationAttributes.Any())
                return null;

            return registrationAttributes.First();
        }

        private static readonly Dictionary<IcmpMessageType, IcmpDatagram> _prototypes = InitializeComplexOptions();
    }
}