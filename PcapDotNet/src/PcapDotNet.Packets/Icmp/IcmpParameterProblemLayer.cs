using System;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// RFC 792.
    /// </summary>
    public sealed class IcmpParameterProblemLayer : IcmpLayer
    {
        /// <summary>
        /// The maximum value that OriginalDatagramLength can take.
        /// </summary>
        public const int OriginalDatagramLengthMaxValue = Byte.MaxValue * sizeof(uint);

        /// <summary>
        /// The pointer identifies the octet of the original datagram's header where the error was detected (it may be in the middle of an option).  
        /// For example, 1 indicates something is wrong with the Type of Service, and (if there are options present) 20 indicates something is wrong with the type code of the first option.
        /// </summary>
        public byte Pointer { get; set; }

        /// <summary>
        /// Length of the padded "original datagram".
        /// Must divide by 4 and cannot exceed OriginalDatagramLengthMaxValue.
        /// </summary>
        public int OriginalDatagramLength
        {
            get { return _originalDatagramLength; }
            set
            {
                if (value % sizeof(uint) != 0)
                    throw new ArgumentOutOfRangeException("value", value, string.Format("Must divide by {0}.", sizeof(uint)));
                if (value > OriginalDatagramLengthMaxValue)
                    throw new ArgumentOutOfRangeException("value", value, string.Format("Must not exceed {0}.", OriginalDatagramLengthMaxValue));
                _originalDatagramLength = value;
            }
        }

        /// <summary>
        /// The value of this field determines the format of the remaining data.
        /// </summary>
        public override IcmpMessageType MessageType
        {
            get { return IcmpMessageType.ParameterProblem; }
        }

        /// <summary>
        /// A value that should be interpreted according to the specific message.
        /// </summary>
        protected override uint Variable
        {
            get { return (uint)((Pointer << 24) | ((OriginalDatagramLength / sizeof(uint)) << 16)); }
        }

        private int _originalDatagramLength = 0;
    }
}