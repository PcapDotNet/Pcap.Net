namespace PcapDotNet.TestUtils
{
    public static class ByteExtensions
    {
        public static bool[] ToBits(this byte b)
        {
            bool[] bits = new bool[8];
            for (int i = 0; i != 8; ++i)
            {
                bits[7 - i] = (b % 2 == 1);
                b /= 2;
            }

            return bits;
        }
    }
}