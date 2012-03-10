using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 2671.
    /// A set of options.
    /// </summary>
    public sealed class DnsOptions : IEquatable<DnsOptions>
    {
        /// <summary>
        /// Empty options.
        /// </summary>
        public static DnsOptions None { get { return _none; } }

        /// <summary>
        /// Constructs options from the given list of options.
        /// The given otpions list should be modified after the call to this constructor.
        /// </summary>
        public DnsOptions(IList<DnsOption> options)
        {
            Options = options.AsReadOnly();
            BytesLength = options.Sum(option => option.Length);
        }

        /// <summary>
        /// Constructs options from the given list of options.
        /// The given otpions list should be modified after the call to this constructor.
        /// </summary>
        public DnsOptions(params DnsOption[] options)
            : this((IList<DnsOption>)options)
        {
        }

        /// <summary>
        /// The list of options.
        /// </summary>
        public ReadOnlyCollection<DnsOption> Options { get; private set; }

        /// <summary>
        /// The total number of bytes the options take.
        /// </summary>
        public int BytesLength { get; private set; }

        /// <summary>
        /// Two options objects are equal if they have the same options in the same order.
        /// </summary>
        public bool Equals(DnsOptions other)
        {
            return other != null &&
                   Options.SequenceEqual(other.Options);
        }

        /// <summary>
        /// Two options objects are equal if they have the same options in the same order.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsOptions);
        }

        /// <summary>
        /// A hash code based on the list of options.
        /// </summary>
        public override int GetHashCode()
        {
            return Options.SequenceGetHashCode();
        }

        internal static DnsOptions Read(DataSegment data)
        {
            List<DnsOption> options = new List<DnsOption>();
            while (data.Length != 0)
            {
                if (data.Length < DnsOption.MinimumLength)
                    return null;
                DnsOptionCode code = (DnsOptionCode)data.ReadUShort(0, Endianity.Big);
                ushort optionDataLength = data.ReadUShort(sizeof(ushort), Endianity.Big);
                
                int optionLength = DnsOption.MinimumLength + optionDataLength;
                if (data.Length < optionLength)
                    return null;
                DnsOption option = DnsOption.CreateInstance(code, data.Subsegment(DnsOption.MinimumLength, optionDataLength));
                if (option == null)
                    return null;
                options.Add(option);

                data = data.Subsegment(optionLength, data.Length - optionLength);
            }

            return new DnsOptions(options);
        }

        internal void Write(byte[] buffer, int offset)
        {
            foreach (DnsOption option in Options)
                option.Write(buffer, ref offset);
        }

        private static readonly DnsOptions _none = new DnsOptions();
    }
}