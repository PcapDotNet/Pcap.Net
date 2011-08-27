using System;

namespace PcapDotNet.Packets.Icmp
{
    /// <summary>
    /// Used to represent an ICMP datagram with an unknown message type.
    /// </summary>
    public sealed class IcmpUnknownDatagram : IcmpDatagram
    {
        /// <summary>
        /// Creates a Layer that represents the datagram to be used with PacketBuilder.
        /// </summary>
        public override ILayer ExtractLayer()
        {
            return new IcmpUnknownLayer
                       {
                           LayerMessageType = (byte)MessageType,
                           LayerCode = Code,
                           Checksum = Checksum,
                           LayerVariable = Variable,
                           Payload = Payload
                       };
        }

        /// <summary>
        /// Unknown ICMP datagrams are always invalid.
        /// </summary>
        protected override bool CalculateIsValid()
        {
            return false;
        }

        /// <summary>
        /// Take only part of the bytes as a datagarm.
        /// </summary>
        /// <param name="buffer">The bytes to take the datagram from.</param>
        /// <param name="offset">The offset in the buffer to start taking the bytes from.</param>
        /// <param name="length">The number of bytes to take.</param>
        internal IcmpUnknownDatagram(byte[] buffer, int offset, int length)
            : base(buffer, offset, length)
        {
        }

        internal override IcmpDatagram CreateInstance(byte[] buffer, int offset, int length)
        {
            throw new InvalidOperationException("Unknown Icmp Datagram can't be a prototype");
        }
    }
}