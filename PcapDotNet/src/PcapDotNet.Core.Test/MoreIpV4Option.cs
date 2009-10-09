using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Core.Test
{
    internal static class MoreIpV4Option
    {
        public static string GetWiresharkString(this IpV4Option option)
        {
            switch (option.OptionType)
            {
                case IpV4OptionType.EndOfOptionList:
                    return "EOL";

                case IpV4OptionType.NoOperation:
                    return "NOP";

                case IpV4OptionType.BasicSecurity:
                    return "Security";

                case IpV4OptionType.LooseSourceRouting:
                    return "Loose source route (" + option.Length + " bytes)";

                case IpV4OptionType.StrictSourceRouting:
                    return "Strict source route (" + option.Length + " bytes)";

                case IpV4OptionType.RecordRoute:
                    return "Record route (" + option.Length + " bytes)";

                case IpV4OptionType.StreamIdentifier:
                    return "Stream identifier: " + ((IpV4OptionStreamIdentifier)option).Identifier;

                case IpV4OptionType.InternetTimestamp:
                    return "Time stamp" + (option.Length < 5 ? " (with option length = " + option.Length + " bytes; should be >= 5)" : ":");

                case IpV4OptionType.TraceRoute:
                    return "Unknown (0x52) (12 bytes)";

                case IpV4OptionType.RouterAlert:
                    ushort routerAlertValue = ((IpV4OptionRouterAlert)option).Value;
                    return "Router Alert: " + ((routerAlertValue != 0)
                                                   ? "Unknown (" + routerAlertValue + ")"
                                                   : "Every router examines packet");

                case IpV4OptionType.QuickStart:
                    IpV4OptionQuickStart quickStart = (IpV4OptionQuickStart)option;

                    StringBuilder quickStartWireshark = new StringBuilder("Quick-Start: ");

                    if (quickStart.Function == IpV4OptionQuickStartFunction.RateRequest)
                        quickStartWireshark.Append("Rate request");
                    else
                        quickStartWireshark.Append("Rate report");

                    quickStartWireshark.Append(", ");

                    if (quickStart.RateKbps == 0)
                        quickStartWireshark.Append("0 bit/s");
                    else if (quickStart.RateKbps < 1024)
                        quickStartWireshark.Append(quickStart.RateKbps + " kbit/s");
                    else if (quickStart.RateKbps < 1024 * 1024)
                        quickStartWireshark.Append(((double)quickStart.RateKbps / 1000) + " Mbit/s");
                    else
                        quickStartWireshark.Append(((double)quickStart.RateKbps / 1000000) + " Gbit/s");

                    if (quickStart.Function == IpV4OptionQuickStartFunction.RateRequest)
                        quickStartWireshark.Append(", QS TTL " + quickStart.Ttl);

                    return quickStartWireshark.ToString();

                case (IpV4OptionType)134:
                    return "Commercial IP security option" + (option.Length >= 10
                                                                  ? string.Empty
                                                                  : " (with option length = " + option.Length + " bytes; should be >= 10)");

                default:
                    if (typeof(IpV4OptionType).GetEnumValues<IpV4OptionType>().Contains(option.OptionType))
                        throw new InvalidOperationException("Invalid option type " + option.OptionType);
                    return "Unknown (0x" + ((byte)option.OptionType).ToString("x2") + ") (" + option.Length + " bytes)";
            }
        }

        public static IEnumerable<string> GetWiresharkSubfieldStrings(this IpV4Option option)
        {
            switch (option.OptionType)
            {
                case IpV4OptionType.EndOfOptionList:
                case IpV4OptionType.NoOperation:
                case IpV4OptionType.StreamIdentifier:
                case IpV4OptionType.RouterAlert:
                case IpV4OptionType.QuickStart:
                    break;

                case IpV4OptionType.LooseSourceRouting:
                case IpV4OptionType.StrictSourceRouting:
                case IpV4OptionType.RecordRoute:
                    IpV4OptionRoute routeOption = (IpV4OptionRoute)option;
                    yield return "Pointer: " + (routeOption.PointedAddressIndex * 4 + 4);
                    for (int i = 0; i != routeOption.Route.Count; ++i)
                        yield return routeOption.Route[i] + (routeOption.PointedAddressIndex == i ? " <- (current)" : string.Empty);
                    break;

                case IpV4OptionType.InternetTimestamp:
                    IpV4OptionTimestamp timestampOption = (IpV4OptionTimestamp)option;
                    if (timestampOption.CountTimestamps == 0)
                        break;

                    yield return "Pointer: " + (timestampOption.PointedIndex * 4 + 5);
                    yield return "Overflow: " + timestampOption.Overflow;
                    switch (timestampOption.TimestampType)
                    {
                        case IpV4OptionTimestampType.TimestampOnly:
                            yield return "Flag: Time stamps only";
                            IpV4OptionTimestampOnly timestampOnlyOption = (IpV4OptionTimestampOnly)option;
                            foreach (IpV4TimeOfDay timeOfDay in timestampOnlyOption.Timestamps)
                                yield return "Time stamp = " + timeOfDay.MillisecondsSinceMidnightUniversalTime;
                            break;

                        case IpV4OptionTimestampType.AddressAndTimestamp:
                            yield return "Flag: Time stamp and address";
                            IpV4OptionTimestampAndAddress timestampAndAddressOption = (IpV4OptionTimestampAndAddress)option;
                            foreach (IpV4OptionTimedAddress timedAddress in timestampAndAddressOption.TimedRoute)
                            {
                                yield return "Address = " + timedAddress.Address + ", " +
                                             "time stamp = " + timedAddress.TimeOfDay.MillisecondsSinceMidnightUniversalTime;
                            }
                            break;

                        case IpV4OptionTimestampType.AddressPrespecified:
                            yield return "Flag: Time stamps for prespecified addresses";
                            IpV4OptionTimestampAndAddress timestampPrespecifiedOption = (IpV4OptionTimestampAndAddress)option;
                            foreach (IpV4OptionTimedAddress timedAddress in timestampPrespecifiedOption.TimedRoute)
                            {
                                yield return "Time stamp = " + timedAddress.Address.ToValue();
                                yield return "Time stamp = " + timedAddress.TimeOfDay.MillisecondsSinceMidnightUniversalTime;
                            }
                            break;

                        default:
                            throw new InvalidOperationException("Illegal timestamp type " + timestampOption.TimestampType);
                    }
                    break;

                case IpV4OptionType.BasicSecurity:
                default:
                    if (typeof(IpV4OptionType).GetEnumValues<IpV4OptionType>().Contains(option.OptionType))
                        throw new InvalidOperationException("Invalid option type " + option.OptionType);
                    break;
            }
        }
    }
}