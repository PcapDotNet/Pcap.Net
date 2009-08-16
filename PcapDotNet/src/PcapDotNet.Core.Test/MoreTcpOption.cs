using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;

namespace PcapDotNet.Core.Test
{
    internal static class MoreTcpOption
    {
        public static string GetWiresharkString(this TcpOption option)
        {
            switch (option.OptionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return "EOL";

                case TcpOptionType.NoOperation:
                    return "NOP";

                default:
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
            }
        }

        public static IEnumerable<string> GetWiresharkSubfieldStrings(this TcpOption option)
        {
            switch (option.OptionType)
            {
                case TcpOptionType.EndOfOptionList:
                case TcpOptionType.NoOperation:
                case TcpOptionType.MaximumSegmentSize:
                    break;

                case TcpOptionType.WindowScale:
                    yield return "";
                    break;

                default:
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
            }
        }
    }
}