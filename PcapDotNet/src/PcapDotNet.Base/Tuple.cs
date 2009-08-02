using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A tuple of two values.
    /// </summary>
    /// <typeparam name="TValue1">The type of the first value.</typeparam>
    /// <typeparam name="TValue2">The type of the second value.</typeparam>
    public struct Tuple<TValue1, TValue2> : IEquatable<Tuple<TValue1, TValue2>>
    {
        /// <summary>
        /// Constructs a tuple from two values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        public Tuple(TValue1 value1, TValue2 value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        /// <summary>
        /// The first value.
        /// </summary>
        public TValue1 Value1
        {
            get { return _value1; }
        }

        /// <summary>
        /// The second value.
        /// </summary>
        public TValue2 Value2
        {
            get { return _value2; }
        }

        /// <summary>
        /// Tuples are equal if all their values are equal.
        /// </summary>
        public bool Equals(Tuple<TValue1, TValue2> other)
        {
            return _value1.Equals(other._value1) &&
                   _value2.Equals(other._value2);
        }

        /// <summary>
        /// Tuples are equal if all their values are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is Tuple<TValue1, TValue2>) &&
                   Equals((Tuple<TValue1, TValue2>)obj);
        }

        /// <summary>
        /// Tuples are equal if all their values are equal.
        /// </summary>
        public static bool operator ==(Tuple<TValue1, TValue2> value1, Tuple<TValue1, TValue2> value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Tuples are equal if all their values are equal.
        /// </summary>
        public static bool operator !=(Tuple<TValue1, TValue2> value1, Tuple<TValue1, TValue2> value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// A hash code for a tuple is a xor its values.
        /// </summary>
        public override int GetHashCode()
        {
            return _value1.GetHashCode() ^ _value2.GetHashCode();
        }

        private readonly TValue1 _value1;
        private readonly TValue2 _value2;
    }
}