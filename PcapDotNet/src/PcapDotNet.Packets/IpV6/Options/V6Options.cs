using System;
using System.Collections.Generic;
using PcapDotNet.Packets.Ip;

namespace PcapDotNet.Packets.IpV6
{
    public abstract class V6Options<T> : Options<T> where T : Option, IEquatable<T>
    {
        public V6Options(IList<T> options, bool isValid) 
            : base(options, isValid, null)
        {
        }

        internal override sealed int CalculateBytesLength(int optionsLength)
        {
            return optionsLength;
        }
    }
}