using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// List of IPv6 options for some of the IPv6 extension headers.
    /// </summary>
    public sealed class IpV6Options : Options<IpV6Option>
    {
        /// <summary>
        /// Empty set of options.
        /// </summary>
        public static IpV6Options Empty { get { return _empty; } }

        /// <summary>
        /// Creates the list of options from a given IList of otpions.
        /// </summary>
        /// <param name="options">List of options to build the set from.</param>
        public IpV6Options(IList<IpV6Option> options)
            : base(options, true, null)
        {
        }

        /// <summary>
        /// Creates the list of options from a given IEnumerable of otpions.
        /// </summary>
        /// <param name="options">List of options to build the set from.</param>
        public IpV6Options(IEnumerable<IpV6Option> options)
            : this(options.ToList())
        {
        }

        /// <summary>
        /// Creates the list of options from a given array of otpions.
        /// </summary>
        /// <param name="options">List of options to build the set from.</param>
        public IpV6Options(params IpV6Option[] options)
            : this((IList<IpV6Option>)options)
        {
        }

        internal IpV6Options(DataSegment data) 
            : this(Read(data))
        {
        }

        internal IpV6Options Pad(int paddingSize)
        {
            if (paddingSize == 0)
                return this;
            IEnumerable<IpV6Option> paddedOptions =
                OptionsCollection.Concat(paddingSize == 1 ? (IpV6Option)new IpV6OptionPad1() : new IpV6OptionPadN(paddingSize - 2));
            return new IpV6Options(new Tuple<IList<IpV6Option>, bool>(paddedOptions.ToList(), IsValid));
        }

        internal static Tuple<IList<IpV6Option>, bool> Read(DataSegment data) 
        {
            int offset = 0;
            List<IpV6Option> options = new List<IpV6Option>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6OptionType optionType = (IpV6OptionType)data[offset++];
                if (optionType == IpV6OptionType.Pad1)
                {
                    options.Add(new IpV6OptionPad1());
                    continue;
                }

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

                IpV6Option option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6Option>, bool>(options, isValid);
        }

        internal override int CalculateBytesLength(int optionsLength)
        {
            return optionsLength;
        }

        private IpV6Options(Tuple<IList<IpV6Option>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2, null)
        {
        }
        
        private static IpV6Option CreateOption(IpV6OptionType optionType, DataSegment data)
        {
            IIpV6OptionComplexFactory factory;
            if (!_factories.TryGetValue(optionType, out factory))
                return new IpV6OptionUnknown(optionType, data);
            return factory.CreateInstance(data);
        }

        private static Dictionary<IpV6OptionType, IIpV6OptionComplexFactory> InitializeFactories()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where GetRegistrationAttribute(type) != null
                select new
                {
                    GetRegistrationAttribute(type).OptionType,
                    Option = (IIpV6OptionComplexFactory)Activator.CreateInstance(type, true)
                };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6OptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6OptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly IpV6Options _empty = new IpV6Options();
        private static readonly Dictionary<IpV6OptionType, IIpV6OptionComplexFactory> _factories = InitializeFactories();
    }
}