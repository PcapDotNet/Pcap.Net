using System;
using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Icmp
{
    public abstract class IcmpLayer : SimpleLayer, IIpV4NextLayer
    {
        public abstract IcmpMessageType MessageType { get; }
        public virtual byte CodeValue
        {
            get { return 0; }
        }
        public IcmpMessageTypeAndCode MessageTypeAndCode 
        {
            get 
            {
                return (IcmpMessageTypeAndCode)(((ushort)MessageType << 8) | CodeValue);
            }
        }

        public ushort? Checksum { get; set; }
        protected virtual uint Value 
        { 
            get { return 0; }
        }

        public override sealed int Length
        {
            get { return IcmpDatagram.HeaderLength + PayloadLength; } 
        }

        protected virtual int PayloadLength
        {
            get { return 0; }
        }

        protected override sealed void Write(byte[] buffer, int offset)
        {
            IcmpDatagram.WriteHeader(buffer, offset, MessageType, CodeValue, Value);
            WritePayload(buffer, offset + IcmpDatagram.HeaderLength);
        }

        protected virtual void WritePayload(byte[] buffer, int offset)
        {
        }

        public override sealed void Finalize(byte[] buffer, int offset, int payloadLength, ILayer nextLayer)
        {
            IcmpDatagram.WriteChecksum(buffer, offset, Length + payloadLength, Checksum);
        }

        public IpV4Protocol PreviousLayerProtocol
        {
            get { return IpV4Protocol.InternetControlMessageProtocol; }
        }

        public virtual bool Equals(IcmpLayer other)
        {
            return other != null &&
                   MessageType == other.MessageType && CodeValue == other.CodeValue &&
                   Checksum == other.Checksum &&
                   Value == other.Value;
        }

        public sealed override bool Equals(Layer other)
        {
            return base.Equals(other) && Equals(other as IcmpLayer);
        }

        public override string ToString()
        {
            return MessageType + "." + CodeValue + "(" + Value + ")";
        }
    }
}