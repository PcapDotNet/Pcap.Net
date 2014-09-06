using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Set of Mobility options for IPv6 Mobility Extension Header.
    /// </summary>
    public class IpV6MobilityOptions : V6Options<IpV6MobilityOption>
    {
        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6MobilityOptions(IList<IpV6MobilityOption> options)
            : base(options, true)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6MobilityOptions(IEnumerable<IpV6MobilityOption> options)
            : this((IpV6MobilityOption[])options.ToArray())
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6MobilityOptions(params IpV6MobilityOption[] options)
            : this((IList<IpV6MobilityOption>)options)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6MobilityOptions None
        {
            get { return _none; }
        }

        internal IpV6MobilityOptions(DataSegment data)
            : this(Read(data))
        {
        }

        internal IpV6MobilityOptions Pad(int paddingSize)
        {
            if (paddingSize == 0)
                return this;
            return new IpV6MobilityOptions(
                OptionsCollection.Concat(paddingSize == 1 ? (IpV6MobilityOption)new IpV6MobilityOptionPad1() : new IpV6MobilityOptionPadN(paddingSize - 2)),
                IsValid);
        }

        private IpV6MobilityOptions(Tuple<IList<IpV6MobilityOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        private IpV6MobilityOptions(IEnumerable<IpV6MobilityOption> options, bool isValid)
            : this(new Tuple<IList<IpV6MobilityOption>, bool>(options.ToList(), isValid))
        {
        }

        private static Tuple<IList<IpV6MobilityOption>, bool> Read(DataSegment data)
        {
            int offset = 0;
            List<IpV6MobilityOption> options = new List<IpV6MobilityOption>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6MobilityOptionType optionType = (IpV6MobilityOptionType)data[offset++];
                if (optionType == IpV6MobilityOptionType.Pad1)
                {
                    options.Add(new IpV6MobilityOptionPad1());
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

                IpV6MobilityOption option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                if (!option.IsValid)
                    isValid = false;

                options.Add(option);
            }

            return new Tuple<IList<IpV6MobilityOption>, bool>(options, isValid);
        }

        private static IpV6MobilityOption CreateOption(IpV6MobilityOptionType optionType, DataSegment data)
        {
            IpV6MobilityOption prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6MobilityOptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static Dictionary<IpV6MobilityOptionType, IpV6MobilityOption> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IpV6MobilityOption).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IpV6MobilityOption)Activator.CreateInstance(type, true)
                           };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6MobilityOptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6MobilityOptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly Dictionary<IpV6MobilityOptionType, IpV6MobilityOption> _prototypes = InitializePrototypes();
        private static readonly IpV6MobilityOptions _none = new IpV6MobilityOptions();
    }
}