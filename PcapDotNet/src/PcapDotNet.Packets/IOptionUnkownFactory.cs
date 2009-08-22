namespace PcapDotNet.Packets
{
    public interface IOptionUnkownFactory<TOptionType>
    {
        Option CreateInstance(TOptionType optionType, byte[] buffer, ref int offset, byte valueLength);
    }
}