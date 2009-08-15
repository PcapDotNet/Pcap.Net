using System;

namespace PcapDotNet.Packets.Transport
{
    public class TcpOptions
    {
        public static TcpOptions None
        {
            get { return _none; }
        }

        /// <summary>
        /// Creates options from a list of options.
        /// </summary>
        /// <param name="options">The list of options.</param>
        public TcpOptions(params TcpOption[] options)
        {
            _bytesLength = 0;
        }

        internal TcpOptions(byte[] buffer, int offset, int length)
//            : this(Read(buffer, offset, length))
        {
            _bytesLength = length;
        }


        public int BytesLength
        {
            get { return _bytesLength; }
        }

        internal void Write(byte[] buffer, int offset)
        {
//            throw new NotImplementedException();
        }

        private readonly int _bytesLength;
        private readonly bool _isValid;
        private static readonly TcpOptions _none = new TcpOptions();
    }
}