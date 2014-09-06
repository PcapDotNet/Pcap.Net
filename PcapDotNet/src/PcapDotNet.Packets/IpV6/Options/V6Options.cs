using System;
using System.Collections.Generic;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// Base class for different IPv6 options types.
    /// </summary>
    /// <typeparam name="T">The option concrete type.</typeparam>
    public abstract class V6Options<T> : Options<T> where T : Option, IEquatable<T>
    {
        internal V6Options(IList<T> options, bool isValid) 
            : base(options, isValid, null)
        {
        }

        internal sealed override int CalculateBytesLength(int optionsLength)
        {
            return optionsLength;
        }
    }
}