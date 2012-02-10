using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    public sealed class DnsOptions : IEquatable<DnsOptions>
    {
        public static DnsOptions None { get { return _none; } }

        public DnsOptions(IList<DnsOption> options)
        {
            Options = options.AsReadOnly();
            BytesLength = options.Sum(option => option.Length);
        }

        public DnsOptions(params DnsOption[] options)
            : this((IList<DnsOption>)options)
        {
        }

        public ReadOnlyCollection<DnsOption> Options { get; private set; }

        public int BytesLength { get; private set; }

        public bool Equals(DnsOptions other)
        {
            return other != null &&
                   Options.SequenceEqual(other.Options);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DnsOptions);
        }

        public override int GetHashCode()
        {
            return Options.SequenceGetHashCode();
        }

        private static readonly DnsOptions _none = new DnsOptions();

        internal void Write(byte[] buffer, int offset)
        {
            foreach (DnsOption option in Options)
                option.Write(buffer, ref offset);
        }

        public static DnsOptions Read(DataSegment data)
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
    }
}