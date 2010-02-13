using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    internal static class OptionComplexFactory<TOptionType>// : OptionComplexFactoryBase
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
            if (optionLength < OptionHeaderLength || length + 1 < optionLength)
                return null;
            byte optionValueLength = (byte)(optionLength - OptionHeaderLength);

            IOptionComplexFactory prototype;
            if (!_complexOptions.TryGetValue(optionType, out prototype))
                return UnknownFactoryOptionPrototype.CreateInstance(optionType, buffer, ref offset, optionValueLength);

            return prototype.CreateInstance(buffer, ref offset, optionValueLength);
        }

        private static IOptionUnknownFactory<TOptionType> InitializeUnknownOptionPrototype()
        {
            var unknownOptions =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IOptionUnknownFactory<TOptionType>).IsAssignableFrom(type)
                select (IOptionUnknownFactory<TOptionType>)Activator.CreateInstance(type);

            if (unknownOptions.Count() != 1)
                throw new InvalidOperationException("Must be only one unknown option for option type " + typeof(TOptionType));

            return unknownOptions.First();
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

//        [DebuggerHidden]
//        private OptionComplexFactory()
//        {
//            throw new NotImplementedException();
//        }

        private static readonly Dictionary<TOptionType, IOptionComplexFactory> _complexOptions = InitializeComplexOptions();
        private static readonly IOptionUnknownFactory<TOptionType> UnknownFactoryOptionPrototype = InitializeUnknownOptionPrototype();
    }
}