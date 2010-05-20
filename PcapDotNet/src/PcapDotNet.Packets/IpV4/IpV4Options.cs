using System.Collections.Generic;

namespace PcapDotNet.Packets.IpV4
{
    /// <summary>
    /// Represents IPv4 Options.
    /// The options may appear or not in datagrams.  
    /// They must be implemented by all IP modules (host and gateways).  
    /// What is optional is their transmission in any particular datagram, not their implementation.
    /// </summary>
    public class IpV4Options : Options<IpV4Option>
    {
        /// <summary>
        /// The maximum number of bytes the options may take.
        /// </summary>
        public const int MaximumBytesLength = IpV4Datagram.HeaderMaximumLength - IpV4Datagram.HeaderMinimumLength;

        /// <summary>
        /// No options instance.
        /// </summary>
        public static IpV4Options None
        {
            get { return _none; }
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV4Options(IList<IpV4Option> options)
            : base(options, IpV4Option.End, MaximumBytesLength)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public IpV4Options(params IpV4Option[] options)
            : this((IList<IpV4Option>)options)
        {
        }

        internal IpV4Options(byte[] buffer, int offset, int length)
            : base(buffer, offset, length, IpV4Option.End)
        {
        }

        private static readonly IpV4Options _none = new IpV4Options();
    }
}