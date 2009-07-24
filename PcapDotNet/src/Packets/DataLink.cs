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

        public bool Equals(DataLink other)
        {
            return Kind == other.Kind;
        }

        public override bool Equals(object obj)
        {
            return (obj is DataLink &&
                    Equals((DataLink)obj));
        }

        public static bool operator ==(DataLink dataLink1, DataLink dataLink2)
        {
            return dataLink1.Equals(dataLink2);
        }

        public static bool operator !=(DataLink dataLink1, DataLink dataLink2)
        {
            return !(dataLink1 == dataLink2);
        }

        public override int GetHashCode()
        {
            return _kind.GetHashCode();
        }

        private readonly DataLinkKind _kind;
    }
}