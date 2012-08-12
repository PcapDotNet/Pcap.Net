using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// A generic Options class.
    /// Represents a list of options (either IPv4 options, IPv6 options or TCP options).
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

        internal void Write(byte[] buffer, int offset)
        {
            int offsetEnd = offset + BytesLength;
            foreach (T option in OptionsCollection)
                option.Write(buffer, ref offset);

            // Padding
            while (offset < offsetEnd)
                buffer[offset++] = 0;
        }

        internal Options(IList<T> options, bool isValid, int? length)
        {
            _options = options.AsReadOnly();

            IsValid = isValid;

            if (length.HasValue)
            {
                BytesLength = length.Value;
            }
            else
            {
                BytesLength = SumBytesLength(OptionsCollection);

                if (BytesLength % 4 != 0)
                    BytesLength = (BytesLength / 4 + 1) * 4;
            }
        }

        internal static int SumBytesLength(IEnumerable<T> options)
        {
            return options.Sum(option => option.Length);
        }

        private readonly ReadOnlyCollection<T> _options;
    }
}