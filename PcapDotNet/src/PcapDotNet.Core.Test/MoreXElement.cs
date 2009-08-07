using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    internal static class MoreXElement
    {
        public static IEnumerable<XElement> Fields(this XElement element)
        {
            return element.Elements("field");
        }
    
        public static string Name(this XElement element)
        {
            return element.Attribute("name").Value;
        }

        public static string Show(this XElement element)
        {
            return element.Attribute("show").Value;
        }

        public static string Value(this XElement element)
        {
            return element.Attribute("value").Value;
        }

        public static void AssertShow(this XElement element, string value)
        {
            Assert.AreEqual(element.Show(), value);
        }

        public static void AssertShowDecimal(this XElement element, bool value)
        {
            element.AssertShowDecimal(value ? 1 : 0);
        }

        public static void AssertShowDecimal(this XElement element, byte value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, short value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ushort value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, int value)
        {
            Assert.AreEqual(element.Show(), value.ToString(), element.Name());
        }

        public static void AssertShowDecimal(this XElement element, uint value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, long value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowDecimal(this XElement element, ulong value)
        {
            Assert.AreEqual(element.Show(), value.ToString());
        }

        public static void AssertShowHex(this XElement element, byte value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(byte)));
        }

        public static void AssertShowHex(this XElement element, short value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(ushort)));
        }

        public static void AssertShowHex(this XElement element, ushort value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(ushort)));
        }

        public static void AssertShowHex(this XElement element, int value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(int)));
        }

        public static void AssertShowHex(this XElement element, uint value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(uint)));
        }

        public static void AssertShowHex(this XElement element, long value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(long)));
        }

        public static void AssertShowHex(this XElement element, ulong value)
        {
            Assert.AreEqual(element.Show(), "0x" + value.ToString("x" + 2 * sizeof(ulong)));
        }
    }
}