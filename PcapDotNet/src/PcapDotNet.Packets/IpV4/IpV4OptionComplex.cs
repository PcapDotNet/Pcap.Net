using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PcapDotNet.Packets.IpV4
{
    internal class IpV4OptionTypeRegistrationAttribute : Attribute
    {
        public IpV4OptionTypeRegistrationAttribute(IpV4OptionType optionType)
        {
            OptionType = optionType;
        }

        public IpV4OptionType OptionType { get; set; }
    }

    internal interface IIpv4OptionComplexFactory
    {
        IpV4OptionComplex CreateInstance(byte[] buffer, ref int offset, byte valueLength);
    }

    /// <summary>
    /// Represents a complex IPv4 option.
    /// Complex option means that it contains data and not just the type.
    /// </summary>
    public abstract class IpV4OptionComplex : IpV4Option
    {
        private static readonly Dictionary<IpV4OptionType, IIpv4OptionComplexFactory> _complexOptions;

        static IpV4OptionComplex()
        {
            var complexOptions =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IIpv4OptionComplexFactory).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IIpv4OptionComplexFactory)Activator.CreateInstance(type)
                           };

            _complexOptions = complexOptions.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV4OptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes(typeof(IpV4OptionTypeRegistrationAttribute), false);
            if (registraionAttributes.Length == 0)
                return null;

            return ((IpV4OptionTypeRegistrationAttribute)registraionAttributes[0]);
        }

        /// <summary>
        /// The header length in bytes for the option (type and size).
        /// </summary>
        public const int OptionHeaderLength = 2;

        internal static IpV4OptionComplex ReadOptionComplex(IpV4OptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 1)
                return null;
            byte optionLength = buffer[offset++];
            if (length + 1 < optionLength)
                return null;
            byte optionValueLength = (byte)(optionLength - OptionHeaderLength);

            IIpv4OptionComplexFactory prototype;
            if (!_complexOptions.TryGetValue(optionType, out prototype))
                return null;

            return prototype.CreateInstance(buffer, ref offset, optionValueLength);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer[offset++] = (byte)Length;
        }

        /// <summary>
        /// Constructs the option by type.
        /// </summary>
        protected IpV4OptionComplex(IpV4OptionType type)
            : base(type)
        {
        }
    }
}