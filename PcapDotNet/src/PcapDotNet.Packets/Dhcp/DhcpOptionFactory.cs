using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PcapDotNet.Packets.Dhcp.Options;

namespace PcapDotNet.Packets.Dhcp
{
    internal static class DhcpOptionFactory
    {
        internal static DhcpOption CreateInstance(DataSegment data, ref int offset)
        {
            DhcpOptionCode optionCode = (DhcpOptionCode)data[offset++];

            MethodInfo readMethod;
            if (_optionReaders.TryGetValue(optionCode, out readMethod))
            {
                object[] args = new object[] { data, offset };
                DhcpOption option = (DhcpOption)readMethod.Invoke(null, args);
                offset = (int)args[1];
                return option;
            }
            else
            {
                return DhcpAnyOption.Read(data, ref offset);
            }
        }

        private static Dictionary<DhcpOptionCode, MethodInfo> InitializeComplexOptions()
        {
            Dictionary<DhcpOptionCode, MethodInfo> optionReaders = new Dictionary<DhcpOptionCode, MethodInfo>();
            foreach (MethodInfo readMethod in Assembly.GetExecutingAssembly().GetTypes().
                Where(type=> typeof(DhcpOption).IsAssignableFrom(type))
                .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)))
            {
                DhcpOptionReadRegistrationAttribute readRegistrationAttribute = readMethod.GetCustomAttribute<DhcpOptionReadRegistrationAttribute>(false);
                if (readRegistrationAttribute == null)
                    continue;

                if (typeof(DataSegment).IsAssignableFrom(readMethod.GetParameters()[0].ParameterType) &&
                    readMethod.GetParameters()[1].ParameterType == typeof(int).MakeByRefType() &&
                    typeof(DhcpOption).IsAssignableFrom(readMethod.ReturnType))
                {
                    optionReaders.Add(readRegistrationAttribute.OptionCode, readMethod);
                }
                else
                {
                    throw new NotSupportedException("Method " + readMethod + " has a DhcpOptionReadRegistrationAttribute but has no valid signature");
                }
            }
            return optionReaders;
        }
        
        private static readonly Dictionary<DhcpOptionCode, MethodInfo> _optionReaders = InitializeComplexOptions();
    }
}
