using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 6553.
    /// Routing Protocol for Low-Power and Lossy Networks option.
    /// <pre>
    /// +-----+---+---+---+-----+---------------+
    /// | Bit | 0 | 1 | 2 | 3-7 | 8-15          |
    /// +-----+---+---+---+-----+---------------+
    /// | 0   | Option Type     | Opt Data Len  |
    /// +-----+---+---+---+-----+---------------+
    /// | 16  | O | R | F | 0   | RPLInstanceID |
    /// +-----+---+---+---+-----+---------------+
    /// | 32  | SenderRank                      |
    /// +-----+---------------------------------+
    /// | 48  | (sub-TLVs)                      |
    /// | ... |                                 |
    /// +-----+---------------------------------+
    /// </pre>
    /// </summary>
    [IpV6OptionTypeRegistration(IpV6OptionType.RplOption)]
    public sealed class IpV6OptionRoutingProtocolLowPowerAndLossyNetworks : IpV6OptionComplex, IIpV6OptionComplexFactory
    {
        private static class Offset
        {
            public const int Down = 0;
            public const int RankError = Down;
            public const int ForwardingError = RankError;
            public const int RplInstanceId = ForwardingError + sizeof(byte);
            public const int SenderRank = RplInstanceId + sizeof(byte);
            public const int SubTlvs = SenderRank + sizeof(ushort);
        }

        private static class Mask
        {
            public const byte Down = 0x80;
            public const byte RankError = 0x40;
            public const byte ForwardingError = 0x20;
        }

        /// <summary>
        /// The minimum number of bytes this option data takes.
        /// </summary>
        public const int OptionDataMinimumLength = Offset.SubTlvs;

        /// <summary>
        /// Creates an instance from down, rank error, forwarding error, RPL instance id, sender rank and sub TLVs.
        /// </summary>
        /// <param name="down">
        /// Indicating whether the packet is expected to progress Up or Down.
        /// A router sets the Down flag when the packet is expected to progress Down (using DAO routes), 
        /// and clears it when forwarding toward the DODAG root (to a node with a lower Rank).
        /// A host or RPL leaf node must set the Down flag to 0.
        /// </param>
        /// <param name="rankError">
        /// Indicating whether a Rank error was detected.
        /// A Rank error is detected when there is a mismatch in the relative Ranks and the direction as indicated in the Down flag.
        /// A host or RPL leaf node must set the Rank Error flag to 0.
        /// </param>
        /// <param name="forwardingError">
        /// Indicating that this node cannot forward the packet further towards the destination.
        /// The Forward Error flag might be set by a child node that does not have a route to destination for a packet with the Down flag set.
        /// A host or RPL leaf node must set the Forwarding error flag to 0.
        /// </param>
        /// <param name="rplInstanceId">Indicating the DODAG instance along which the packet is sent.</param>
        /// <param name="senderRank">Set to zero by the source and to DAGRank(rank) by a router that forwards inside the RPL network.</param>
        /// <param name="subTlvs">
        /// A RPL device must skip over any unrecognized sub-TLVs and attempt to process any additional sub-TLVs that may appear after.
        /// </param>
        public IpV6OptionRoutingProtocolLowPowerAndLossyNetworks(bool down, bool rankError, bool forwardingError, byte rplInstanceId, ushort senderRank,
                                                                 DataSegment subTlvs)
            : base(IpV6OptionType.RplOption)
        {
            Down = down;
            RankError = rankError;
            ForwardingError = forwardingError;
            RplInstanceId = rplInstanceId;
            SenderRank = senderRank;
            SubTlvs = subTlvs;
        }

        /// <summary>
        /// Indicating whether the packet is expected to progress Up or Down.
        /// A router sets the Down flag when the packet is expected to progress Down (using DAO routes), 
        /// and clears it when forwarding toward the DODAG root (to a node with a lower Rank).
        /// A host or RPL leaf node must set the Down flag to 0.
        /// </summary>
        public bool Down { get; private set; }

        /// <summary>
        /// Indicating whether a Rank error was detected.
        /// A Rank error is detected when there is a mismatch in the relative Ranks and the direction as indicated in the Down flag.
        /// A host or RPL leaf node must set the Rank Error flag to 0.
        /// </summary>
        public bool RankError { get; private set; }

        /// <summary>
        /// Indicating that this node cannot forward the packet further towards the destination.
        /// The Forward Error flag might be set by a child node that does not have a route to destination for a packet with the Down flag set.
        /// A host or RPL leaf node must set the Forwarding error flag to 0.
        /// </summary>
        public bool ForwardingError { get; private set; }

        /// <summary>
        /// Indicating the DODAG instance along which the packet is sent.
        /// </summary>
        public byte RplInstanceId { get; private set; }

        /// <summary>
        /// Set to zero by the source and to DAGRank(rank) by a router that forwards inside the RPL network.
        /// </summary>
        public ushort SenderRank{ get; private set; }

        /// <summary>
        /// A RPL device must skip over any unrecognized sub-TLVs and attempt to process any additional sub-TLVs that may appear after.
        /// </summary>
        public DataSegment SubTlvs { get; private set; }

        /// <summary>
        /// Parses an option from the given data.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The option if parsing was successful, null otherwise.</returns>
        public IpV6Option CreateInstance(DataSegment data)
        {
            if (data.Length < OptionDataMinimumLength)
                return null;

            bool down = data.ReadBool(Offset.Down, Mask.Down);
            bool rankError = data.ReadBool(Offset.RankError, Mask.RankError);
            bool forwardingError = data.ReadBool(Offset.ForwardingError, Mask.ForwardingError);
            byte rplInstanceId = data[Offset.RplInstanceId];
            ushort senderRank = data.ReadUShort(Offset.SenderRank, Endianity.Big);
            DataSegment subTlvs = data.Subsegment(Offset.SubTlvs, data.Length - Offset.SubTlvs);

            return new IpV6OptionRoutingProtocolLowPowerAndLossyNetworks(down, rankError, forwardingError, rplInstanceId, senderRank, subTlvs);
        }

        internal override int DataLength
        {
            get { return OptionDataMinimumLength + SubTlvs.Length; }
        }

        internal override bool EqualsData(IpV6Option other)
        {
            return EqualsData(other as IpV6OptionRoutingProtocolLowPowerAndLossyNetworks);
        }

        internal override int GetDataHashCode()
        {
            return Sequence.GetHashCode(BitSequence.Merge(BitSequence.Merge(Down, RankError, ForwardingError), RplInstanceId, SenderRank), SubTlvs);
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            byte flags = (byte)((Down ? Mask.Down : 0) | (RankError ? Mask.RankError : 0) | (ForwardingError ? Mask.ForwardingError : 0));
            buffer.Write(ref offset, flags);
            buffer.Write(ref offset, RplInstanceId);
            buffer.Write(ref offset, SenderRank, Endianity.Big);
            buffer.Write(ref offset, SubTlvs);
        }

        private IpV6OptionRoutingProtocolLowPowerAndLossyNetworks()
            : this(false, false, false, 0, 0, DataSegment.Empty)
        {
        }

        private bool EqualsData(IpV6OptionRoutingProtocolLowPowerAndLossyNetworks other)
        {
            return other != null &&
                   Down == other.Down && RankError == other.RankError && ForwardingError == other.ForwardingError && RplInstanceId == other.RplInstanceId &&
                   SenderRank == other.SenderRank && SubTlvs.Equals(SubTlvs);
        }
    }
}