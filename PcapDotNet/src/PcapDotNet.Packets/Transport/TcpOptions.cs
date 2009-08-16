using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Transport
{
    public class TcpOptions : Options<TcpOption>
    {
        public const int MaximumBytesLength = TcpDatagram.HeaderMaximumLength - TcpDatagram.HeaderMinimumLength;

        public static TcpOptions None
        {
            get { return _none; }
        }

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