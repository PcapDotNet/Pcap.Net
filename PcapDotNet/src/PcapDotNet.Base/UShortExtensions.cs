using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension method for UShort structure.
    /// </summary>
    public static class UShortExtensions
    {
        public static ushort ReverseEndianity(this ushort value)
        {
            return (ushort)((short)value).ReverseEndianity();
        }
    }
}