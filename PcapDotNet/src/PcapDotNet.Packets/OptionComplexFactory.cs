using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class OptionComplexFactory<TOptionType>
    {
        /// <summary>
        /// The header length in bytes for the option (type and size).
        /// </summary>
        public const int OptionHeaderLength = 2;

        internal static Option Read(TOptionType optionType, byte[] buffer, ref int offset, int length)
        {
            if (length < 1)
                return null;
            byte optionLength = buffer[offset++];
            if (length + 1 < optionLength)
                return null;
            byte optionValueLength = (byte)(optionLength - OptionHeaderLength);

            IOptionComplexFactory prototype;
            if (!_complexOptions.TryGetValue(optionType, out prototype))
                return null;

            return prototype.CreateInstance(buffer, ref offset, optionValueLength);
        }

        private static Dictionary<TOptionType, IOptionComplexFactory> InitializeComplexOptions()
        {
            var complexOptions =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IOptionComplexFactory).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IOptionComplexFactory)Activator.CreateInstance(type)
                           };

            return complexOptions.ToDictionary(option => (TOptionType)option.OptionType, option => option.Option);
        }

        private static OptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes =
                from attribute in type.GetCustomAttributes(typeof(OptionTypeRegistrationAttribute), false).Cast<OptionTypeRegistrationAttribute>()
                where attribute.OptionTypeType == typeof(TOptionType)
                select attribute;

            if (registraionAttributes.IsEmpty())
                return null;

            return registraionAttributes.First();
        }

        private static readonly Dictionary<TOptionType, IOptionComplexFactory> _complexOptions = InitializeComplexOptions();
    }
}