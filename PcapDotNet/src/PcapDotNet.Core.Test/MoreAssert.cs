using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.Core.Test
{
    internal static class MoreAssert
    {
        public static void IsBigger<T>(T expectedMinimum, T actual) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actual) >= 0)
                throw new AssertFailedException("Assert.IsBigger failed. Expected minimum: <" + expectedMinimum +
                                                "> Actual: <" + actual + ">.");
        }

        public static void IsSmaller<T>(T expectedMaximum, T actual) where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actual) <= 0)
                throw new AssertFailedException("Assert.IsSmaller failed. Expected maximum: <" + expectedMaximum +
                                                "> Actual: <" + actual + ">.");
        }

        public static void IsBiggerOrEqual<T>(T expectedMinimum, T actual) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actual) > 0)
                throw new AssertFailedException("Assert.IsBiggerOrEqual failed. Expected minimum: <" + expectedMinimum +
                                                "> Actual: <" + actual + ">.");
        }

        public static void IsSmallerOrEqual<T>(T expectedMaximum, T actual) where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actual) < 0)
                throw new AssertFailedException("Assert.IsSmallerOrEqual failed. Expected maximum: <" + expectedMaximum +
                                                "> Actual: <" + actual + ">.");
        }

        public static void IsInRange<T>(T expectedMinimum, T expectedMaximum, T actual) where T : IComparable<T>
        {
            IsBiggerOrEqual(expectedMinimum, actual);
            IsSmallerOrEqual(expectedMaximum, actual);
        }

        public static void IsMatch(string expectedPattern, string actualValue)
        {
            Assert.IsTrue(Regex.IsMatch(actualValue, expectedPattern), "Expected pattern: <" + expectedPattern + ">. Actual value: <" + actualValue + ">.");
        }
    }
}