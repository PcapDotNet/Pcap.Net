using System;

namespace PcapDotNet.Packets
{
    public abstract class IpV4Option : IEquatable<IpV4Option>
    {
        public static IpV4OptionSimple End
        {
            get { return _end; }
        }

        public static IpV4OptionSimple Nop
        {
            get { return _nop; }
        }

        public IpV4OptionType OptionType
        {
            get { return _type; }
        }

        public abstract int Length
        { 
            get;
        }

        public abstract bool IsAppearsAtMostOnce
        {
            get;
        }

        public bool Equivalent(IpV4Option option)
        {
            return OptionType == option.OptionType;
        }

        public virtual bool Equals(IpV4Option other)
        {
            if (other == null)
                return false;
            return Equivalent(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IpV4Option);
        }

        public override int GetHashCode()
        {
            return (byte)OptionType;
        }

        public override string ToString()
        {
            return OptionType.ToString();
        }

        protected IpV4Option(IpV4OptionType type)
        {
            _type = type;
        }

        internal static IpV4Option Read(byte[] buffer, ref int offset, int length)
        {
            int offsetEnd = offset + length;
            if (offset == offsetEnd)
                return null;

            IpV4OptionType optionType = (IpV4OptionType)buffer[offset++];
            switch (optionType)
            {
                case IpV4OptionType.EndOfOptionList:
                    return End;
                case IpV4OptionType.NoOperation:
                    return Nop;

                case IpV4OptionType.Security:
                case IpV4OptionType.LooseSourceRouting:
                case IpV4OptionType.StrictSourceRouting:
                case IpV4OptionType.RecordRoute:
                case IpV4OptionType.StreamIdentifier:
                case IpV4OptionType.InternetTimestamp:
                    return IpV4OptionComplex.ReadOptionComplex(optionType, buffer, ref offset, offsetEnd - offset);

                default:
                    return null;
            }
        }

        internal virtual void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        private static readonly IpV4OptionSimple _end = new IpV4OptionSimple(IpV4OptionType.EndOfOptionList);
        private static readonly IpV4OptionSimple _nop = new IpV4OptionSimple(IpV4OptionType.NoOperation);

        private readonly IpV4OptionType _type;
    }
}