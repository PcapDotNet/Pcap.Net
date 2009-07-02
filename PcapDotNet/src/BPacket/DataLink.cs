namespace Packets
{
    public struct DataLink : IDataLink
    {
        public DataLink(DataLinkKind kind)
        {
            _kind = kind;
        }

        public DataLinkKind Kind
        {
            get { return _kind; }
        }

        private readonly DataLinkKind _kind;
    }
}