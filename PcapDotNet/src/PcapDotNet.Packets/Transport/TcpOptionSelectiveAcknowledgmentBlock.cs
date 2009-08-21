namespace PcapDotNet.Packets.Transport
{
    public struct TcpOptionSelectiveAcknowledgmentBlock
    {
        public const int SizeOf = 8;

        public TcpOptionSelectiveAcknowledgmentBlock(uint leftEdge, uint rightEdge)
        {
            _leftEdge = leftEdge;
            _rightEdge = rightEdge;
        }

        public uint LeftEdge 
        {
            get { return _leftEdge; }
        }

        public uint RightEdge
        {
            get { return _rightEdge; }
        }

        public override string ToString()
        {
            return LeftEdge + "-" + RightEdge;
        }

        private readonly uint _leftEdge;
        private readonly uint _rightEdge;
    }
}