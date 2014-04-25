using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Core.Test
{
    internal static class XElementExtensions
    {
        public static IEnumerable<XElement> Fields(this XElement element)
        {
            return element.Elements("field");
        }

        public static IEnumerable<XElement> Protocols(this XElement element)
        {
            return element.Elements("proto");
        }

        public static string GetAttributeValue(this XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null)
                return string.Empty;

            return attribute.Value;
        }
    
        public static string Name(this XElement element)
        {
            return element.GetAttributeValue("name");
        }

        public static string Show(this XElement element)
        {
            return element.GetAttributeValue("show");
        }

        public static string Showname(this XElement element)
        {
            return element.GetAttributeValue("showname");
        }

        public static string Value(this XElement element)
        {
            return element.GetAttributeValue("value");
        }

        public static void AssertName(this XElement element, string expectedName)
        {
            Assert.AreEqual(element.Name(), expectedName);
        }

        public static void AssertNoFields(this XElement element)
        {
            Assert.IsFalse(element.Fields().Any(), string.Format("Element {0} has {1} fields.", element.Name(), element.Fields().Count()));
        }

        public static void AssertNumFields(this XElement element, int expectedNumFields)
        {
            Assert.AreEqual(expectedNumFields, element.Fields().Count());
        }

        public static void AssertNoShow(this XElement element)
        {
            Assert.IsNull(element.Attribute("show"));
        }

        public static void AssertShow(this XElement element, string expectedValue, bool ignoreCase = false, string message = null)
        {
            Assert.AreEqual(expectedValue, element.Show(), ignoreCase, message ?? element.Name());
        }

        public static void AssertShow(this XElement element, string expectedValue, string message)
        {
            element.AssertShow(expectedValue, false, message);
        }

        public static void AssertShow(this XElement element, IEnumerable<byte> expectedValue)
        {
            element.AssertShow(expectedValue.BytesSequenceToHexadecimalString(":"));
        }

        public static void AssertShowDecimal(this XElement element, bool expectedValue)
        {
            element.AssertShowDecimal(expectedValue ? 1 : 0);
        }

        public static void AssertShowDecimal(this XElement element, byte expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowDecimal(this XElement element, short expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ushort expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowDecimal(this XElement element, int expectedValue, string message = null)
        {
            element.AssertShow(expectedValue.ToString(), message);
        }

        public static void AssertShowDecimal(this XElement element, uint expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowDecimal(this XElement element, long expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ulong expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
        }

        public static void AssertShowHex(this XElement element, byte expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(byte)));
        }

        public static void AssertShowHex(this XElement element, short expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(short)));
        }

        public static void AssertShowHex(this XElement element, ushort expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(ushort)));
        }

        public static void AssertShowHex(this XElement element, int expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(int)));
        }

        public static void AssertShowHex(this XElement element, uint expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(uint)));
        }

        public static void AssertShowHex(this XElement element, long expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(long)));
        }

        public static void AssertShowHex(this XElement element, ulong expectedValue)
        {
            element.AssertShow("0x" + expectedValue.ToString("x" + 2 * sizeof(ulong)));
        }

        public static void AssertValue(this XElement element, string expectedValue, string message = null)
        {
            Assert.AreEqual(expectedValue.Length, element.Value().Length, message ?? element.Name());
            Assert.AreEqual(expectedValue, element.Value(), message ?? element.Name());
        }

        public static void AssertValueInRange(this XElement element, string expectedMinimumValue, string expectedMaximumValue)
        {
            MoreAssert.IsInRange(expectedMinimumValue, expectedMaximumValue, element.Value());
        }

        public static void AssertValue(this XElement element, IEnumerable<byte> expectedValue, string message = null)
        {
            element.AssertValue(expectedValue.BytesSequenceToHexadecimalString(), message);
        }

        public static void AssertValue(this XElement element, byte expectedValue)
        {
            element.AssertValue(expectedValue.ToString("x2"));
        }

        public static void AssertValue(this XElement element, ushort expectedValue, string message = null)
        {
            element.AssertValue(expectedValue.ToString("x4"), message);
        }

        public static void AssertValue(this XElement element, uint expectedValue)
        {
            element.AssertValue(expectedValue.ToString("x8"));
        }

        public static void AssertValueInRange(this XElement element, uint expectedMinimumValue, uint expectedMaximumValue)
        {
            element.AssertValueInRange(expectedMinimumValue.ToString("x8"), expectedMaximumValue.ToString("x8"));
        }

        public static void AssertValue(this XElement element, UInt48 expectedValue)
        {
            element.AssertValue(expectedValue.ToString("x12"));
        }

        public static void AssertValue(this XElement element, ulong expectedValue)
        {
            element.AssertValue(expectedValue.ToString("x16"));
        }

        public static void AssertValue(this XElement element, SerialNumber32 expectedValue)
        {
            element.AssertValue(expectedValue.Value);
        }

        public static void AssertValue(this XElement element, IEnumerable<string> expectedValue)
        {
            element.AssertValue(expectedValue.SequenceToString());
        }

        public static void AssertValue(this XElement element, IEnumerable<ushort> expectedValue)
        {
            element.AssertValue(expectedValue.Select(value => value.ToString("x4")));
        }

        public static void AssertValue(this XElement element, IEnumerable<uint> expectedValue)
        {
            element.AssertValue(expectedValue.Select(value => value.ToString("x8")));
        }

        public static void AssertValue(this XElement element, IEnumerable<IpV4Address> expectedValue)
        {
            element.AssertValue(expectedValue.Select(ip => ip.ToValue()));
        }

        public static void AssertDataField(this XElement element, string expectedValue)
        {
            element.AssertName("data");
            element.AssertValue(expectedValue);

            element.AssertNumFields(2);

            var dataData = element.Fields().First();
            dataData.AssertName("data.data");
            dataData.AssertValue(expectedValue);

            var dataLen = element.Fields().Last();
            dataLen.AssertName("data.len");
            dataLen.AssertShowDecimal(expectedValue.Length / 2);
        }

        public static void AssertDataField(this XElement element, IEnumerable<byte> expectedValue)
        {
            element.AssertDataField(expectedValue.BytesSequenceToHexadecimalString());
        }
    }
}