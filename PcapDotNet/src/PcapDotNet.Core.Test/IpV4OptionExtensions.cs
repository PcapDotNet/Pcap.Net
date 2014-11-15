using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Core.Test
{
    internal static class IpV4OptionExtensions
    {
        public static bool IsBadForWireshark(this IpV4Options options)
        {
            // TODO: This shouldn't be a factor once https://bugs.wireshark.org/bugzilla/show_bug.cgi?id=7043 is fixed.
            return options.OptionsCollection.Any(option => option.OptionType == IpV4OptionType.InternetTimestamp && option.Length < 5 ||
                                                           option.OptionType == IpV4OptionType.BasicSecurity && option.Length != 11);
        }
    }
}