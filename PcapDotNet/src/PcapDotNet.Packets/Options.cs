using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// A generic Options class.
    /// Represents a list of options (either IPv4 options or TCP options).
    /// </summary>
    /// <typeparam name="T">The Option type this collection contains.</typeparam>
    public abstract class Options<T> where T : Option
    {
        /// <summary>
        /// Returns the collection of options.
        /// </summary>
        public ReadOnlyCollection<T> OptionsCollection
        {
            get { return _options; }
        }

        /// <summary>
        /// Returns the number of options.
        /// </summary>
        public int Count
        {
            get { return OptionsCollection.Count; }
        }

        /// <summary>
        /// Returns the option in the given index.
        /// </summary>
        /// <param name="index">The zero based index of the option.</param>
        /// <returns>The option in the given index.</returns>
        public T this[int index]
        {
            get { return OptionsCollection[index]; }
        }

        /// <summary>
        /// The number of bytes the options take.
        /// </summary>
        public int BytesLength { get; private set; }

        /// <summary>
        /// Whether or not the options parsed ok.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Two options are equal iff they have the exact same options.
        /// </summary>
        public bool Equals(Options<T> other)
        {
            if (other == null)
                return false;

            if (BytesLength != other.BytesLength)
                return false;

            return OptionsCollection.SequenceEqual(other.OptionsCollection);
        }

        /// <summary>
        /// Two options are equal iff they have the exact same options.
        /// </summary>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as Options<T>);
        }

        /// <summary>
        /// The hash code is the xor of the following hash codes: number of bytes the options take and all the options.
        /// </summary>
        public sealed override int GetHashCode()
        {
            return BytesLength.GetHashCode() ^
                   OptionsCollection.SequenceGetHashCode();
        }

        /// <summary>
        /// A string of all the option type names.
        /// </summary>
        public sealed override string ToString()
        {
            return OptionsCollection.SequenceToString(", ", GetType().Name + " {", "}");
        }

        internal Options(byte[] buffer, int offset, int length, T end)
            : this(Read(buffer, offset, length, end))
        {
            BytesLength = length;
        }

        internal void Write(byte[] buffer, int offset)
        {
            int offsetEnd = offset + BytesLength;
            foreach (T option in OptionsCollection)
                option.Write(buffer, ref offset);

            // Padding
            while (offset < offsetEnd)
                buffer[offset++] = 0;
        }

        internal Options(IList<T> options, T end, int maximumBytesLength)
            : this(EndOptions(options, end), true)
        {
            if (BytesLength > maximumBytesLength)
                throw new ArgumentException("given options take " + BytesLength + " bytes and maximum number of bytes for options is " + maximumBytesLength, "options");
        }

        private Options(IList<T> options, bool isValid)
        {
            _options = new ReadOnlyCollection<T>(options);

            IsValid = isValid;

            BytesLength = SumBytesLength(OptionsCollection);

            if (BytesLength % 4 != 0)
                BytesLength = (BytesLength / 4 + 1) * 4;
        }

        private static IList<T> EndOptions(IList<T> options, T end)
        {
            if (options.Count == 0 || options.Last().Equivalent(end) || SumBytesLength(options) % 4 == 0)
                return options;

            return new List<T>(options.Concat(end));
        }

        private static int SumBytesLength(IEnumerable<T> options)
        {
            return options.Sum(option => option.Length);
        }

        private Options(Tuple<IList<T>, bool> optionsAndIsValid)
            : this(optionsAndIsValid.Item1, optionsAndIsValid.Item2)
        {
        }

        private static Tuple<IList<T>, bool> Read(byte[] buffer, int offset, int length, T end)
        {
            int offsetEnd = offset + length;

            List<T> options = new List<T>();
            while (offset != offsetEnd)
            {
                T option = (T)end.Read(buffer, ref offset, offsetEnd - offset);
                if (option == null ||
                    option.IsAppearsAtMostOnce && options.Any(option.Equivalent))
                {
                    // Invalid
                    return new Tuple<IList<T>, bool>(options, false);
                }

                options.Add(option);
                if (option.Equivalent(end))
                    break; // Valid?
            }

            return new Tuple<IList<T>, bool>(options, true);
        }

        private readonly ReadOnlyCollection<T> _options;
    }
}