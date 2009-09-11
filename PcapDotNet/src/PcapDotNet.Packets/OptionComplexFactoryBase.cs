namespace PcapDotNet.Packets
{
    internal abstract class OptionComplexFactoryBase
    {
        /// <summary>
        /// The header length in bytes for the option (type and size).
        /// </summary>
        public const int OptionHeaderLength = 2;

        protected OptionComplexFactoryBase()
        {
        }
    }
}