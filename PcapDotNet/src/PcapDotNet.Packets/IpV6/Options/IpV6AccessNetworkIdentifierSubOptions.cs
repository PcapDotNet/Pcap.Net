using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6757.
    /// </summary>
    public class IpV6AccessNetworkIdentifierSubOptions : V6Options<IpV6AccessNetworkIdentifierSubOption>
    {
        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6AccessNetworkIdentifierSubOptions(IList<IpV6AccessNetworkIdentifierSubOption> options)
            : base(options, true)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6AccessNetworkIdentifierSubOptions(params IpV6AccessNetworkIdentifierSubOption[] options)
            : this((IList<IpV6AccessNetworkIdentifierSubOption>)options)
        {
        }

        internal IpV6AccessNetworkIdentifierSubOptions(DataSegment data)
            : this(Read(data))
        {
        }

        private IpV6AccessNetworkIdentifierSubOptions(Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6AccessNetworkIdentifierSubOptions None
        {
            get { return _none; }
        }

        internal static Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool> Read(DataSegment data)
        {
            int offset = 0;
            List<IpV6AccessNetworkIdentifierSubOption> options = new List<IpV6AccessNetworkIdentifierSubOption>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6AccessNetworkIdentifierSubOptionType optionType = (IpV6AccessNetworkIdentifierSubOptionType)data[offset++];
                if (offset >= data.Length)
                {
                    isValid = false;
                    break;
                }

                byte optionDataLength = data[offset++];
                if (offset + optionDataLength > data.Length)
                {
                    isValid = false;
                    break;
                }

                IpV6AccessNetworkIdentifierSubOption option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6AccessNetworkIdentifierSubOption>, bool>(options, isValid);
        }

        private static IpV6AccessNetworkIdentifierSubOption CreateOption(IpV6AccessNetworkIdentifierSubOptionType optionType, DataSegment data)
        {
            IpV6AccessNetworkIdentifierSubOption prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6AccessNetworkIdentifierSubOptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6AccessNetworkIdentifierSubOptionType, IpV6AccessNetworkIdentifierSubOption> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6AccessNetworkIdentifierSubOptionType, IpV6AccessNetworkIdentifierSubOption> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IpV6AccessNetworkIdentifierSubOption).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IpV6AccessNetworkIdentifierSubOption)Activator.CreateInstance(type, true)
                           };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6AccessNetworkIdentifierSubOptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly IpV6AccessNetworkIdentifierSubOptions _none = new IpV6AccessNetworkIdentifierSubOptions();
    }
}