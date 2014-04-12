using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6089.
    /// </summary>
    public sealed class IpV6FlowIdentificationSubOptions : V6Options<IpV6FlowIdentificationSubOption>
    {
        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6FlowIdentificationSubOptions(IList<IpV6FlowIdentificationSubOption> options)
            : base(options, true)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV6FlowIdentificationSubOptions(params IpV6FlowIdentificationSubOption[] options)
            : this((IList<IpV6FlowIdentificationSubOption>)options)
        {
        }

        internal IpV6FlowIdentificationSubOptions(DataSegment data)
            : this(Read(data))
        {
        }

        private IpV6FlowIdentificationSubOptions(Tuple<IList<IpV6FlowIdentificationSubOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6FlowIdentificationSubOptions None
        {
            get { return _none; }
        }

        internal static Tuple<IList<IpV6FlowIdentificationSubOption>, bool> Read(DataSegment data)
        {
            int offset = 0;
            List<IpV6FlowIdentificationSubOption> options = new List<IpV6FlowIdentificationSubOption>();
            bool isValid = true;
            while (offset < data.Length)
            {
                IpV6FlowIdentificationSubOptionType optionType = (IpV6FlowIdentificationSubOptionType)data[offset++];
                if (optionType == IpV6FlowIdentificationSubOptionType.Pad1)
                {
                    options.Add(new IpV6FlowIdentificationSubOptionPad1());
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

                IpV6FlowIdentificationSubOption option = CreateOption(optionType, data.Subsegment(ref offset, optionDataLength));
                if (option == null)
                {
                    isValid = false;
                    break;
                }

                options.Add(option);
            }

            return new Tuple<IList<IpV6FlowIdentificationSubOption>, bool>(options, isValid);
        }

        private static IpV6FlowIdentificationSubOption CreateOption(IpV6FlowIdentificationSubOptionType optionType, DataSegment data)
        {
            IpV6FlowIdentificationSubOption prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6FlowIdentificationSubOptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6FlowIdentificationSubOptionType, IpV6FlowIdentificationSubOption> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6FlowIdentificationSubOptionType, IpV6FlowIdentificationSubOption> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof (IpV6FlowIdentificationSubOption).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IpV6FlowIdentificationSubOption)Activator.CreateInstance(type, true)
                           };

            return prototypes.ToDictionary(option => option.OptionType, option => option.Option);
        }

        private static IpV6FlowIdentificationSubOptionTypeRegistrationAttribute GetRegistrationAttribute(Type type)
        {
            var registraionAttributes = type.GetCustomAttributes<IpV6FlowIdentificationSubOptionTypeRegistrationAttribute>(false);
            if (!registraionAttributes.Any())
                return null;

            return registraionAttributes.First();
        }

        private static readonly IpV6FlowIdentificationSubOptions _none = new IpV6FlowIdentificationSubOptions();
    }
}