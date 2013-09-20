using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    internal interface IIpV6OptionComplexFactory
    {
        IpV6Option CreateInstance(DataSegment data);
    }

    public sealed class IpV6Options : Options<IpV6Option>
    {
        public IpV6Options(IList<IpV6Option> options)
            : base(AddPadding(options), true, null)
        {
        }

        public IpV6Options(IEnumerable<IpV6Option> options)
            : this(options.ToList())
        {
        }

        public IpV6Options(DataSegment data) 
            : this(Read(data))
        {
        }

        private IpV6Options(Tuple<IList<IpV6Option>, bool> optionsAndIsValid)
            : base(AddPadding(optionsAndIsValid.Item1), optionsAndIsValid.Item2, null)
        {
        }

        private static IList<IpV6Option> AddPadding(IList<IpV6Option> options)
        {
            int optionsLength = options.Sum(option => option.Length);
            if (optionsLength % 8 == 6)
                return options;

            int paddingLength = (14 - optionsLength % 8) % 8;
            return options.Concat(paddingLength == 1 ? (IpV6Option)new IpV6OptionPad1() : new IpV6OptionPadN(paddingLength - 2)).ToArray();
        }

        public static Tuple<IList<IpV6Option>, bool> Read(DataSegment data) 
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

        private static IpV6Option CreateOption(IpV6OptionType optionType, DataSegment data)
        {
            IIpV6OptionComplexFactory factory;
            if (!_factories.TryGetValue(optionType, out factory))
                return new IpV6OptionUnknown(optionType, data);
            return factory.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6OptionType, IIpV6OptionComplexFactory> _factories = InitializeFactories();

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
    }

    public abstract class V6Options<T> : Options<T> where T : Option
    {
        public V6Options(IList<T> options, bool isValid) 
            : base(options, isValid, null)
        {
        }

        internal override sealed int CalculateBytesLength(int optionsLength)
        {
            return optionsLength;
        }
    }

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
        public IpV6MobilityOptions(params IpV6MobilityOption[] options)
            : this((IList<IpV6MobilityOption>)options)
        {
        }

        internal IpV6MobilityOptions(DataSegment data)
            : this(Read(data))
        {
        }

        private IpV6MobilityOptions(Tuple<IList<IpV6MobilityOption>, bool> optionsAndIsValid)
            : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV6MobilityOptions None
        {
            get { return _none; }
        }

        public static Tuple<IList<IpV6MobilityOption>, bool> Read(DataSegment data)
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

        private static readonly Dictionary<IpV6MobilityOptionType, IpV6MobilityOption> _prototypes = InitializePrototypes();

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

        private static readonly IpV6MobilityOptions _none = new IpV6MobilityOptions();
    }
}