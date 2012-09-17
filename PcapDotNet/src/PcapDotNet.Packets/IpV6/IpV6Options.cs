using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    public class IpV6Options : Options<IpV6Option>
    {
        public IpV6Options(DataSegment data) : this(Read(data))
        {
        }

        private IpV6Options(Tuple<IList<IpV6Option>, bool> optionsAndIsValid) : base(optionsAndIsValid.Item1, optionsAndIsValid.Item2, null)
        {
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

        private static IpV6Option CreateOption(IpV6OptionType optionType, DataSegment data)
        {
            IpV6Option prototype;
            if (!_prototypes.TryGetValue(optionType, out prototype))
                return new IpV6OptionUnknown(optionType, data);
            return prototype.CreateInstance(data);
        }

        private static readonly Dictionary<IpV6OptionType, IpV6Option> _prototypes = InitializePrototypes();

        private static Dictionary<IpV6OptionType, IpV6Option> InitializePrototypes()
        {
            var prototypes =
                from type in Assembly.GetExecutingAssembly().GetTypes()
                where typeof(IpV6Option).IsAssignableFrom(type) &&
                      GetRegistrationAttribute(type) != null
                select new
                           {
                               GetRegistrationAttribute(type).OptionType,
                               Option = (IpV6Option)Activator.CreateInstance(type)
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
}