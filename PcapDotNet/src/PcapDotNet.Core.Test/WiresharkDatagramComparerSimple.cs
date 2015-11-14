using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using PcapDotNet.Packets;

namespace PcapDotNet.Core.Test
{
    [ExcludeFromCodeCoverage]
    internal abstract class WiresharkDatagramComparerSimple : WiresharkDatagramComparer
    {
        protected sealed override bool CompareField(XElement field, Datagram parentDatagram, Datagram datagram)
        {
            return CompareField(field, datagram);
        }

        protected abstract bool CompareField(XElement field, Datagram datagram);
    }
}