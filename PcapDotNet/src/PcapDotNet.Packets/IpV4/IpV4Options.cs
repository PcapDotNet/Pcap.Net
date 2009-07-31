using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets
{
    public class IpV4Options : IEquatable<IpV4Options>
    {
        public const int MaximumLength = IpV4Datagram.HeaderMaximumLength - IpV4Datagram.HeaderMinimumLength;

        public static IpV4Options None
        {
            get { return _none; }
        }

        public IpV4Options(IEnumerable<IpV4Option> options)
        {
            _options.AddRange(options);
            foreach (IpV4Option option in _options)
                _length += option.Length;

            if (_length % 4 != 0)
                _length = (_length / 4 + 1) * 4;
        }

        public IpV4Options(params IpV4Option[] options)
            : this((IEnumerable<IpV4Option>)options)
        {
        }

        internal IpV4Options()
        {
        }

        internal IpV4Options(byte[] buffer, int offset, int length)
        {
            _length = length;

            int offsetEnd = offset + length;
            while (offset != offsetEnd)
            {
                IpV4Option option = IpV4Option.Read(buffer, ref offset, offsetEnd - offset);
                if (option == null)
                    return; // Invalid

                if (option.IsAppearsAtMostOnce && _options.FindIndex(option.Equivalent) != -1)
                {
                    return; // Invalid
                }

                _options.Add(option);
                if (option is IpV4OptionEndOfOptionsList)
                    break; // Valid?
            }
        }

        public int Length
        {
            get { return _length; }
        }

        public bool Equals(IpV4Options other)
        {
            if (other == null)
                return false;

            if (Length != other.Length)
                return false;

            return _options.SequenceEqual(other._options);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV4Options);
        }

        internal void Write(byte[] buffer, int offset)
        {
            int offsetEnd = offset + Length;
            foreach (IpV4Option option in _options)
                option.Write(buffer, ref offset);

            // Padding
            while (offset < offsetEnd)
                buffer[offset++] = 0;
        }

        private readonly List<IpV4Option> _options = new List<IpV4Option>();
        private readonly int _length;
        private static readonly IpV4Options _none = new IpV4Options();
    }
}