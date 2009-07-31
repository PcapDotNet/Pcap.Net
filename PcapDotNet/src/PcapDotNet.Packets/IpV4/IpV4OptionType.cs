namespace Packets
{
    public enum IpV4OptionType : byte
    {
        EndOfOptionList = 0,
        NoOperation = 1,
        Security = 130,
        LooseSourceRouting = 131,
        StrictSourceRouting = 137,
        RecordRoute = 7,
        StreamIdentifier = 136,
        InternetTimestamp = 68
    }
}