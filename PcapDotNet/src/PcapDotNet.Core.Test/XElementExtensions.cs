using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Base;

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
                throw new ArgumentException("element " + element.Name + " doesn't contain attribute " + attributeName, "attributeName");

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

        public static string Value(this XElement element)
        {
            return element.GetAttributeValue("value");
        }

        public static void AssertName(this XElement element, string expectedName)
        {
            Assert.AreEqual(element.Name(), expectedName);
        }

        public static void AssertShow(this XElement element, string expectedValue)
        {
            Assert.AreEqual(expectedValue, element.Show(), element.Name());
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

        public static void AssertShowDecimal(this XElement element, int expectedValue)
        {
            element.AssertShow(expectedValue.ToString());
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
            Assert.AreEqual(element.Value(), expectedValue, message ?? element.Name());
        }

        public static void AssertValue(this XElement element, IEnumerable<byte> expectedValue)
        {
            element.AssertValue(expectedValue.BytesSequenceToHexadecimalString());
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
    }
}