using System;

namespace Packets
{
    public abstract class IpV4Option : IEquatable<IpV4Option>
    {
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
                    return new IpV4OptionEndOfOptionsList();

                case IpV4OptionType.NoOperation:
                    return new IpV4OptionNoOperation();

                case IpV4OptionType.Security:
                    return IpV4OptionSecurity.ReadOptionSecurity(buffer, ref offset, offsetEnd - offset);

                case IpV4OptionType.LooseSourceRouting:
                    return IpV4OptionLooseSourceRouting.ReadOptionLooseSourceRouting(buffer, ref offset, offsetEnd - offset);

                case IpV4OptionType.StrictSourceRouting:
                    return IpV4OptionStrictSourceRouting.ReadOptionStrictSourceRouting(buffer, ref offset, offsetEnd - offset);

                case IpV4OptionType.RecordRoute:
                    return IpV4OptionRecordRoute.ReadOptionRecordRoute(buffer, ref offset, offsetEnd - offset);

                case IpV4OptionType.StreamIdentifier:
                    return IpV4OptionStreamIdentifier.ReadOptionStreamIdentifier(buffer, ref offset, offsetEnd - offset);

                case IpV4OptionType.InternetTimestamp:
                    return IpV4OptionTimestamp.ReadOptionTimestamp(buffer, ref offset, offsetEnd - offset);

                default:
                    return null;
            }
        }

        internal virtual void Write(byte[] buffer, ref int offset)
        {
            buffer[offset++] = (byte)OptionType;
        }

        private readonly IpV4OptionType _type;
    }
}