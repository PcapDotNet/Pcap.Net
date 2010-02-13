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

        public static void AssertShow(this XElement element, string value)
        {
            Assert.AreEqual(element.Show(), value, element.Name());
        }

        public static void AssertShow(this XElement element, IEnumerable<byte> value)
        {
            element.AssertShow(value.BytesSequenceToHexadecimalString(":"));
        }

        public static void AssertShowDecimal(this XElement element, bool value)
        {
            element.AssertShowDecimal(value ? 1 : 0);
        }

        public static void AssertShowDecimal(this XElement element, byte value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, short value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ushort value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, int value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, uint value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, long value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ulong value)
        {
            element.AssertShow(value.ToString());
        }

        public static void AssertShowHex(this XElement element, byte value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(byte)));
        }

        public static void AssertShowHex(this XElement element, short value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(short)));
        }

        public static void AssertShowHex(this XElement element, ushort value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(ushort)));
        }

        public static void AssertShowHex(this XElement element, int value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(int)));
        }

        public static void AssertShowHex(this XElement element, uint value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(uint)));
        }

        public static void AssertShowHex(this XElement element, long value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(long)));
        }

        public static void AssertShowHex(this XElement element, ulong value)
        {
            element.AssertShow("0x" + value.ToString("x" + 2 * sizeof(ulong)));
        }

        public static void AssertValue(this XElement element, string value)
        {
            Assert.AreEqual(element.Value(), value, element.Name());
        }

        public static void AssertValue(this XElement element, IEnumerable<byte> bytes)
        {
            element.AssertValue(bytes.BytesSequenceToHexadecimalString());
        }
    }
}