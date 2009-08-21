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
        TimeStamp = 8,
        PartialOrderConnectionPermitted = 9,
        PartialOrderServiceProfile = 10,
        Cc = 11,
        CcNew = 12,
        CcEcho = 13,
        AlternateChecksumRequest = 14,
        AlternateChecksumData = 15,
        Md5Signature = 19,
        QuickStartResponse = 27,
        UserTimeout = 28,
    }
}