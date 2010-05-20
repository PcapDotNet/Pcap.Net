using System.Collections.Generic;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// A collection of TCP options.
    /// </summary>
    public class TcpOptions : Options<TcpOption>
    {
        /// <summary>
        /// The maximum number of bytes the options can take.
        /// </summary>
        public const int MaximumBytesLength = TcpDatagram.HeaderMaximumLength - TcpDatagram.HeaderMinimumLength;

        /// <summary>
        /// An empty options collection.
        /// </summary>
        public static TcpOptions None
        {
            get { return _none; }
        }

        /// <summary>
        /// Creates the options collection from the given list of options.
        /// </summary>
        public TcpOptions(IList<TcpOption> options)
            : base(options, TcpOption.End, MaximumBytesLength)
        {
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public TcpOptions(params TcpOption[] options)
            : this((IList<TcpOption>)options)
        {
        }

        internal TcpOptions(byte[] buffer, int offset, int length)
            : base(buffer, offset, length, TcpOption.End)
        {
        }


        private static readonly TcpOptions _none = new TcpOptions();
    }
}