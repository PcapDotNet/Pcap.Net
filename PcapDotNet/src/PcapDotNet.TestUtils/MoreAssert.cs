using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PcapDotNet.TestUtils
{
    public static class MoreAssert
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

        public static void IsBiggerOrEqual<T>(T expectedMinimum, T actual, string message) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actual) > 0)
                throw new AssertFailedException("Assert.IsBiggerOrEqual failed. Expected minimum: <" + expectedMinimum +
                                                "> Actual: <" + actual + ">. " + message);
        }

        public static void IsBiggerOrEqual<T>(T expectedMaximum, T actual) where T : IComparable<T>
        {
            IsBiggerOrEqual(expectedMaximum, actual, string.Empty);
        }

        public static void IsSmallerOrEqual<T>(T expectedMaximum, T actual, string message) where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actual) < 0)
                throw new AssertFailedException("Assert.IsSmallerOrEqual failed. Expected maximum: <" + expectedMaximum +
                                                "> Actual: <" + actual + ">. " + message);
        }

        public static void IsSmallerOrEqual<T>(T expectedMaximum, T actual) where T : IComparable<T>
        {
            IsSmallerOrEqual(expectedMaximum, actual, string.Empty);
        }

        public static void IsInRange<T>(T expectedMinimum, T expectedMaximum, T actual, string message) where T : IComparable<T>
        {
            IsBiggerOrEqual(expectedMinimum, actual, message);
            IsSmallerOrEqual(expectedMaximum, actual, message);
        }

        public static void IsInRange<T>(T expectedMinimum, T expectedMaximum, T actual) where T : IComparable<T>
        {
            IsInRange(expectedMinimum, expectedMaximum, actual, string.Empty);
        }

        public static void IsMatch(string expectedPattern, string actualValue)
        {
            Assert.IsTrue(Regex.IsMatch(actualValue, expectedPattern), "Expected pattern: <" + expectedPattern + ">. Actual value: <" + actualValue + ">.");
        }

        public static void AreSequenceEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence, string message)
        {
            if (expectedSequence.SequenceEqual(actualSequence))
                return;

//            message = "Expected sequence: <" + expectedSequence.SequenceToString(",") + ">. Actual sequence: <" +
//                      actualSequence.SequenceToString(",") + ">. " + message;

            Assert.AreEqual(expectedSequence.Count(), actualSequence.Count(), "Different Count. " + message);
            List<T> expectedList = expectedSequence.ToList();
            List<T> actualList = actualSequence.ToList();
            for (int i = 0; i != expectedList.Count; ++i)
                Assert.AreEqual(expectedList[i], actualList[i], "Element " + (i + 1) + " is different in the sequence. " + message);

            Assert.Fail(message);
        }

        public static void AreSequenceEqual<T>(IEnumerable<T> expectedSequence, IEnumerable<T> actualSequence)
        {
            AreSequenceEqual(expectedSequence, actualSequence, string.Empty);
        }
    }
}