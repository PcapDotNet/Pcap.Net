using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets
{
    public class IpV4Options : ReadOnlyCollection<IpV4Option>, IEquatable<IpV4Options>
    {
        public const int MaximumLength = IpV4Datagram.HeaderMaximumLength - IpV4Datagram.HeaderMinimumLength;

        public static IpV4Options None
        {
            get { return _none; }
        }

        public IpV4Options(IList<IpV4Option> options)
            : this(EndOptions(options), true)
        {
        }

        public IpV4Options(params IpV4Option[] options)
            : this((IList<IpV4Option>)options)
        {
        }

        public int BytesLength
        {
            get { return _bytesLength; }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public bool Equals(IpV4Options other)
        {
            if (other == null)
                return false;

            if (BytesLength != other.BytesLength)
                return false;

            return this.SequenceEqual(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV4Options);
        }

        public override int GetHashCode()
        {
            return BytesLength.GetHashCode() ^
                   this.Aggregate(0, (value, option) => value ^ option.GetHashCode());
        }

        public override string ToString()
        {
            return this.SequenceToString(", ", typeof(IpV4Options).Name + " {", "}");
        }
        
        internal IpV4Options(byte[] buffer, int offset, int length)
            : this(Read(buffer, offset, length))
        {
            _bytesLength = length;
        }

        private IpV4Options(IList<IpV4Option> options, bool isValid)
            : base(options)
        {
            _isValid = isValid;

            _bytesLength = SumBytesLength(this);

            if (_bytesLength % 4 != 0)
                _bytesLength = (_bytesLength / 4 + 1) * 4;
        }

        private static IList<IpV4Option> EndOptions(IList<IpV4Option> options)
        {
            if (options.Count == 0 || options.Last().Equals(IpV4Option.End) || SumBytesLength(options) % 4 == 0)
                return options;

            return new List<IpV4Option>(options.Concat(IpV4Option.End));
        }

        private static int SumBytesLength(IEnumerable<IpV4Option> options)
        {
            return options.Sum(option => option.Length);
        }

        private IpV4Options(Tuple<IList<IpV4Option>, bool> optionsAndIsValid)
            : this(optionsAndIsValid.Value1, optionsAndIsValid.Value2)
        {
        }

        private static Tuple<IList<IpV4Option>, bool> Read(byte[] buffer, int offset, int length)
        {
            int offsetEnd = offset + length;

            List<IpV4Option> options = new List<IpV4Option>();
            while (offset != offsetEnd)
            {
                IpV4Option option = IpV4Option.Read(buffer, ref offset, offsetEnd - offset);
                if (option == null || 
                    option.IsAppearsAtMostOnce && options.Any(option.Equivalent))
                {
                    // Invalid
                    return new Tuple<IList<IpV4Option>, bool>(options, false);
                }

                options.Add(option);
                if (option.OptionType == IpV4OptionType.EndOfOptionList)
                    break; // Valid?
            }

            return new Tuple<IList<IpV4Option>, bool>(options, true);
        }

        internal void Write(byte[] buffer, int offset)
        {
            int offsetEnd = offset + BytesLength;
            foreach (IpV4Option option in this)
                option.Write(buffer, ref offset);

            // Padding
            while (offset < offsetEnd)
                buffer[offset++] = 0;
        }

        private readonly int _bytesLength;
        private readonly bool _isValid;
        private static readonly IpV4Options _none = new IpV4Options();
    }
}