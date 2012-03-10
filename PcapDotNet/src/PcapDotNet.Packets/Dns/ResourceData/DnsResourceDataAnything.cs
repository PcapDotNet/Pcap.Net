using System;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// A resource data that can hold any data.
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.Null)]
    [DnsTypeRegistration(Type = DnsType.EId)]
    [DnsTypeRegistration(Type = DnsType.NimrodLocator)]
    [DnsTypeRegistration(Type = DnsType.DynamicHostConfigurationId)]
    public sealed class DnsResourceDataAnything : DnsResourceDataSimple, IEquatable<DnsResourceDataAnything>
    {
        /// <summary>
        /// An empty resource data.
        /// </summary>
        public DnsResourceDataAnything()
        {
            Data = DataSegment.Empty;
        }

        /// <summary>
        /// Constructs the resource data from the given data.
        /// </summary>
        public DnsResourceDataAnything(DataSegment data)
        {
            Data = data;
        }

        /// <summary>
        /// The data of the resource data.
        /// </summary>
        public DataSegment Data { get; private set; }

        /// <summary>
        /// Two resource datas are equal if their data is equal.
        /// </summary>
        public bool Equals(DnsResourceDataAnything other)
        {
            return other != null && Data.Equals(other.Data);
        }

        /// <summary>
        /// Two resource datas are equal if their data is equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as DnsResourceDataAnything);
        }

        /// <summary>
        /// The hash code of the data.
        /// </summary>
        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        internal override int GetLength()
        {
            return Data.Length;
        }

        internal override void WriteDataSimple(byte[] buffer, int offset)
        {
            Data.Write(buffer, offset);
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            return new DnsResourceDataAnything(data);
        }
    }
}