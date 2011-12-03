using System;
using System.Text;
using System.Xml.Linq;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Icmp;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerIcmp : WiresharkDatagramComparerSimple
    {
        protected override string PropertyName
        {
            get { return "Icmp"; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IcmpDatagram icmpDatagram = (IcmpDatagram)datagram;
            switch (field.Name())
            {
                case "icmp.type":
                    field.AssertShowDecimal((byte)icmpDatagram.MessageType);
                    break;

                case "icmp.code":
                    field.AssertShowDecimal(icmpDatagram.Code);
                    break;

                case "icmp.checksum_bad":
                    field.AssertShowDecimal(!icmpDatagram.IsChecksumCorrect);
                    break;

                case "icmp.checksum":
                    field.AssertShowHex(icmpDatagram.Checksum);
                    break;

                case "data":
                    var casted1 = icmpDatagram as IcmpIpV4HeaderPlus64BitsPayloadDatagram;
                    if (casted1 != null)
                        field.AssertValue(casted1.IpV4.Payload);
                    else
                        field.AssertValue(icmpDatagram.Payload);
                    break;

                case "data.data":
                    var casted2 = icmpDatagram as IcmpIpV4HeaderPlus64BitsPayloadDatagram;
                    if (casted2 != null)
                        field.AssertShow(casted2.IpV4.Payload);
                    else
                        field.AssertShow(icmpDatagram.Payload);
                    break;

                case "data.len":
                    var casted3 = icmpDatagram as IcmpIpV4HeaderPlus64BitsPayloadDatagram;
                    if (casted3 != null)
                        field.AssertShowDecimal(casted3.IpV4.Payload.Length);
                    else
                        field.AssertShowDecimal(icmpDatagram.Payload.Length);

                    break;

                case "":
                    switch (icmpDatagram.MessageType)
                    {
                        case IcmpMessageType.ParameterProblem:
                            if (field.Show() != "Unknown session type")
                                field.AssertShow("Pointer: " + ((IcmpParameterProblemDatagram)icmpDatagram).Pointer);
                            break;

                        case IcmpMessageType.RouterAdvertisement:
                            IcmpRouterAdvertisementDatagram routerAdvertisementDatagram = (IcmpRouterAdvertisementDatagram)icmpDatagram;
                            string fieldName = field.Show().Split(':')[0];
                            switch (fieldName)
                            {
                                case "Number of addresses":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.NumberOfAddresses);
                                    break;

                                case "Address entry size":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.AddressEntrySize);
                                    break;

                                case "Lifetime":
                                    TimeSpan actualLifetime = routerAdvertisementDatagram.Lifetime;
                                    StringBuilder actualLifetimeString = new StringBuilder(fieldName + ": ");
                                    if (actualLifetime.Hours != 0)
                                    {
                                        actualLifetimeString.Append(actualLifetime.Hours + " hour");
                                        if (actualLifetime.Hours != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    if (actualLifetime.Minutes != 0)
                                    {
                                        if (actualLifetime.Hours != 0)
                                            actualLifetimeString.Append(", ");
                                        actualLifetimeString.Append(actualLifetime.Minutes + " minute");
                                        if (actualLifetime.Minutes != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    if (actualLifetime.Seconds != 0)
                                    {
                                        if (actualLifetime.Hours != 0 || actualLifetime.Minutes != 0)
                                            actualLifetimeString.Append(", ");
                                        actualLifetimeString.Append(actualLifetime.Seconds + " second");
                                        if (actualLifetime.Seconds != 1)
                                            actualLifetimeString.Append('s');
                                    }
                                    break;

                                case "Router address":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.Entries[_routerIndex].RouterAddress);
                                    break;

                                case "Preference level":
                                    field.AssertShow(fieldName + ": " + routerAdvertisementDatagram.Entries[_routerIndex++].RouterAddressPreference);
                                    break;

                                default:
                                    throw new InvalidOperationException("Invalid icmp " + icmpDatagram.MessageType + " field " + fieldName);
                            }
                            break;
                    }
                    break;

                case "icmp.ident":
                    field.AssertShowHex(((IcmpIdentifiedDatagram)icmpDatagram).Identifier);
                    break;

                case "icmp.seq":
                    field.AssertShowDecimal(((IcmpIdentifiedDatagram)icmpDatagram).SequenceNumber);
                    break;

                case "icmp.seq_le":
                    byte[] sequenceNumberBuffer = new byte[sizeof(ushort)];
                    sequenceNumberBuffer.Write(0, ((IcmpIdentifiedDatagram)icmpDatagram).SequenceNumber, Endianity.Big);
                    ushort lowerEndianSequenceNumber = sequenceNumberBuffer.ReadUShort(0, Endianity.Small);
                    field.AssertShowDecimal(lowerEndianSequenceNumber);
                    break;

                case "icmp.redir_gw":
                    field.AssertShow(((IcmpRedirectDatagram)icmpDatagram).GatewayInternetAddress.ToString());
                    break;

                case "icmp.mtu":
                    field.AssertShowDecimal(((IcmpDestinationUnreachableDatagram)icmpDatagram).NextHopMaximumTransmissionUnit);
                    break;

                default:
                    if (!field.Name().StartsWith("lt2p.") &&
                        !field.Name().StartsWith("pweth."))
                        throw new InvalidOperationException("Invalid icmp field " + field.Name());
                    break;
            }

            return true;
        }

        private int _routerIndex;
    }
}