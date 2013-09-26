using System;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Ip
{
    public abstract class V4Options<T> : Options<T> where T : V4Option, IEquatable<T>
    {
        internal V4Options(byte[] buffer, int offset, int length, T end)
            : this(Read(buffer, offset, length, end), length)
        {
        }

        internal V4Options(IList<T> options, T end, int maximumBytesLength)
            : this(EndOptions(options, end), true, null)
        {
            if (BytesLength > maximumBytesLength)
                throw new ArgumentException("given options take " + BytesLength + " bytes and maximum number of bytes for options is " + maximumBytesLength, "options");
        }

        internal override sealed int CalculateBytesLength(int optionsLength)
        {
            if (optionsLength % 4 == 0)
                return optionsLength;
            return (optionsLength / 4 + 1) * 4;
        }

        private V4Options(Tuple<IList<T>, bool> optionsAndIsValid, int? length)
            : this(optionsAndIsValid.Item1, optionsAndIsValid.Item2, length)
        {
        }

        private V4Options(IList<T> options, bool isValid, int? length)
            :base(options, isValid, length)
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

        private static IList<T> EndOptions(IList<T> options, T end)
        {
            if (options.Count == 0 || options.Last().Equivalent(end) || SumBytesLength(options) % 4 == 0)
                return options;

            return new List<T>(options.Concat(end));
        }
    }
}