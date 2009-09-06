namespace PcapDotNet.Packets.Transport
{
    public enum TcpOptionType : byte
    {
        EndOfOptionList = 0,
        NoOperation = 1,
        MaximumSegmentSize = 2,
        WindowScale = 3,
        SelectiveAcknowledgmentPermitted = 4,
        SelectiveAcknowledgment = 5,
        Echo = 6,
        EchoReply = 7,
        Timestamp = 8,
        PartialOrderConnectionPermitted = 9,
        PartialOrderServiceProfile = 10,
        ConnectionCount = 11,
        ConnectionCountNew = 12,
        ConnectionCountEcho = 13,
        AlternateChecksumRequest = 14,
        AlternateChecksumData = 15,
        Md5Signature = 19,
        QuickStartResponse = 27,
        UserTimeout = 28,
    }
}