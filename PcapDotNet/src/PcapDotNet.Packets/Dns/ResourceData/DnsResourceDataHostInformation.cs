using System.Collections.Generic;

namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// RFC 1035.
    /// <pre>
    /// +-----+
    /// | CPU |
    /// +-----+ 
    /// | OS  |
    /// +-----+
    /// </pre>
    /// </summary>
    [DnsTypeRegistration(Type = DnsType.HInfo)]
    public sealed class DnsResourceDataHostInformation : DnsResourceDataStrings
    {
        private const int NumStrings = 2;

        /// <summary>
        /// Constructs the resource data from the CPU and OS parameters.
        /// </summary>
        /// <param name="cpu">A string which specifies the CPU type.</param>
        /// <param name="os">A string which specifies the operating system type.</param>
        public DnsResourceDataHostInformation(DataSegment cpu, DataSegment os)
            : base(cpu, os)
        {
        }

        /// <summary>
        /// A string which specifies the CPU type.
        /// </summary>
        public DataSegment Cpu { get { return Strings[0]; } }

        /// <summary>
        /// A string which specifies the operating system type.
        /// </summary>
        public DataSegment Os { get { return Strings[1]; } }

        internal DnsResourceDataHostInformation()
            : this(DataSegment.Empty, DataSegment.Empty)
        {
        }

        internal override DnsResourceData CreateInstance(DataSegment data)
        {
            List<DataSegment> strings = ReadStrings(data, NumStrings);
            if (strings == null || strings.Count != NumStrings)
                return null;

            return new DnsResourceDataHostInformation(strings[0], strings[1]);
        }
    }
}