using System.Xml.Linq;
using PcapDotNet.Packets;

namespace PcapDotNet.Core.Test
{
    internal abstract class WiresharkDatagramComparerSimple : WiresharkDatagramComparer
    {
        protected override sealed bool CompareField(XElement field, Datagram parentDatagram, Datagram datagram)
        {
            return CompareField(field, datagram);
        }

        protected abstract bool CompareField(XElement field, Datagram datagram);
    }
}