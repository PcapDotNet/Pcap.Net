namespace Packets
{
    public class Datagram
    {
        public Datagram(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {
        }

        public Datagram(byte[] buffer, int offset, int length)
        {
            _buffer = buffer;
            _startOffset = offset;
            _length = length;
        }

        public static Datagram Empty
        {
            get { return _empty; }
        }

        public int Length
        {
            get { return _length; }
        }

        internal void Write(byte[] buffer, int offset)
        {
            System.Buffer.BlockCopy(_buffer, StartOffset, buffer, offset, Length);
        }

        protected byte[] Buffer
        {
            get { return _buffer; }
        }

        protected int StartOffset
        {
            get { return _startOffset; }
        }

        protected ushort ReadUShort(int offset, Endianity endianity)
        {
            return _buffer.ReadUShort(StartOffset + offset, endianity);
        }

        private static Datagram _empty = new Datagram(new byte[0], 0, 0);
        private byte[] _buffer;
        private int _startOffset;
        private int _length;
    }
}