using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.IpV6;

namespace PcapDotNet.Core.Test
{
    internal class WiresharkDatagramComparerIpV6AuthenticationHeader : WiresharkDatagramComparerSimple
    {
        public WiresharkDatagramComparerIpV6AuthenticationHeader(int count)
        {
            _count = count;
        }

        protected override string PropertyName
        {
            get { return ""; }
        }

        protected override bool CompareField(XElement field, Datagram datagram)
        {
            IpV6Datagram ipV6Datagram = datagram as IpV6Datagram;
            if (ipV6Datagram == null)
                return true;
            while (_count > 0)
            {
                do
                {
                    ++_currentExtensionHeaderIndex;
                } while (ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex].Protocol != IpV4Protocol.AuthenticationHeader);
                --_count;
            }
            IpV6ExtensionHeaderAuthentication authenticationHeader = (IpV6ExtensionHeaderAuthentication)ipV6Datagram.ExtensionHeaders[_currentExtensionHeaderIndex];

            switch (field.Name())
            {
                case "":
                    string[] headerFieldShowParts = field.Show().Split(':');
                    string headerFieldShowName = headerFieldShowParts[0];
                    string headerFieldShowValue = headerFieldShowParts[1];
                    switch (headerFieldShowName)
                    {
                        case "Next Header":
                            field.AssertValue((byte)authenticationHeader.NextHeader.Value);
                            break;

                        case "Length":
                            Assert.AreEqual(string.Format(" {0}", authenticationHeader.Length), headerFieldShowValue);
                            break;

                        default:
                            throw new InvalidOperationException("Invalid ipv6 authentication header unnamed field show name " + headerFieldShowName);
                    }
                    break;

                case "ah.spi":
                    field.AssertShowHex(authenticationHeader.SecurityParametersIndex);
                    break;

                case "ah.sequence":
                    field.AssertShowDecimal(authenticationHeader.SequenceNumber);
                    break;

                case "ah.icv":
                    field.AssertValue(authenticationHeader.AuthenticationData);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Invalid ipv6 authentication header field {0}", field.Name()));
            }

            return true;
        }

        private int _currentExtensionHeaderIndex = -1;
        private int _count;
    }
}