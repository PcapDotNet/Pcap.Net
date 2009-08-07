using System;
using System.Collections.Generic;
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

                case IpV4OptionType.Security:
                    return "Security:";

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

                default:
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
            }
        }

        public static IEnumerable<string> GetWiresharkSubfieldStrings(this IpV4Option option)
        {
            switch (option.OptionType)
            {
                case IpV4OptionType.EndOfOptionList:
                case IpV4OptionType.NoOperation:
                case IpV4OptionType.StreamIdentifier:
                    break;

                case IpV4OptionType.Security:
                    IpV4OptionSecurity securityOption = (IpV4OptionSecurity)option;
                    switch (securityOption.Level)
                    {
                        case IpV4OptionSecurityLevel.EncryptedForTransmissionOnly:
                            yield return "Security: EFTO";
                            break;

                        case IpV4OptionSecurityLevel.Mmmm:
                            yield return "Security: " + securityOption.Level.ToString().ToUpper();
                            break;

                        case IpV4OptionSecurityLevel.TopSecret:
                            yield return "Security: Top secret";
                            break;

                        case IpV4OptionSecurityLevel.Prog:
                            yield return "Security: Unknown (0x" + ((ushort)securityOption.Level).ToString("x4") + ")";
                            break;

                        default:
                            yield return "Security: " + securityOption.Level;
                            break;
                    }

                    yield return "Compartments: " + securityOption.Compartments;
                    yield return "Handling restrictions: ";
                    yield return "Transmission control code: ";
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
                            foreach (IpV4OptionTimeOfDay timeOfDay in timestampOnlyOption.Timestamps)
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

                default:
                    throw new InvalidOperationException("Illegal option type " + option.OptionType);
            }
        }
    }
}