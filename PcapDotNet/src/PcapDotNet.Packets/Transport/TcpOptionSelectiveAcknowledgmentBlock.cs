namespace PcapDotNet.Packets.Transport
{
    public struct TcpOptionSelectiveAcknowledgmentBlock
    {
        public const int SizeOf = 8;

        public TcpOptionSelectiveAcknowledgmentBlock(uint leftEdge, uint rightEdge)
            : this()
        {
            LeftEdge = leftEdge;
            RightEdge = rightEdge;
        }

        public uint LeftEdge 
        {
            get; 
            private set;
        }

        public uint RightEdge
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return LeftEdge + "-" + RightEdge;
        }

        /// <summary>
        /// Two blocks are equal if the have the same left and right edges.
        /// </summary>
        public bool Equals(TcpOptionSelectiveAcknowledgmentBlock other)
        {
            return LeftEdge == other.LeftEdge &&
                   RightEdge == other.RightEdge;
        }

        /// <summary>
        /// Two blocks are equal if the have the same left and right edges.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is TcpOptionSelectiveAcknowledgmentBlock &&
                    Equals((TcpOptionSelectiveAcknowledgmentBlock)obj));
        }

        /// <summary>
        /// Two blocks are equal if the have the same left and right edges.
        /// </summary>
        public static bool operator ==(TcpOptionSelectiveAcknowledgmentBlock value1, TcpOptionSelectiveAcknowledgmentBlock value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Two blocks are different if the have the different left or right edge.
        /// </summary>
        public static bool operator !=(TcpOptionSelectiveAcknowledgmentBlock value1, TcpOptionSelectiveAcknowledgmentBlock value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// The hash code of a block is the xor between the hash code of left and right edges.
        /// </summary>
        public override int GetHashCode()
        {
            return LeftEdge.GetHashCode() ^ RightEdge.GetHashCode();
        }
    }
}