using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets
{
    public class PacketBuilder
    {
        public static Packet Build(DateTime timestamp, params ILayer[] layers)
        {
            return new PacketBuilder(layers).Build(timestamp);
        }

        public static Packet Build(DateTime timestamp, IEnumerable<ILayer> layers)
        {
            return new PacketBuilder(layers).Build(timestamp);
        }

        public PacketBuilder(params ILayer[] layers)
        {
            if (layers.Length == 0)
                throw new ArgumentException("At least one layer must be given", "layers");

            DataLinkKind? dataLinkKind = layers[0].DataLink;
            if (dataLinkKind == null)
                throw new ArgumentException("First layer (" + layers[0].GetType() + ") must provide a DataLink", "layers");

            _layers = layers;
            _dataLink = new DataLink(dataLinkKind.Value);
        }

        public PacketBuilder(IEnumerable<ILayer> layers)
            :this(layers.ToArray())
        {
        }

        public Packet Build(DateTime timestamp)
        {
            int length = _layers.Select(layer => layer.Length).Sum();
            byte[] buffer = new byte[length];

            WriteLayers(buffer, length);
            FinalizeLayers(buffer, length);

            return new Packet(buffer, timestamp, _dataLink);
        }

        private void WriteLayers(byte[] buffer, int length)
        {
            int offset = 0;
            for (int i = 0; i != _layers.Length; ++i)
            {
                ILayer layer = _layers[i];
                ILayer previousLayer = i == 0 ? null : _layers[i - 1];
                ILayer nextLayer = i == _layers.Length - 1 ? null : _layers[i + 1];
                layer.Write(buffer, offset, length - offset - layer.Length, previousLayer, nextLayer);
                offset += layer.Length;
            }
        }

        private void FinalizeLayers(byte[] buffer, int length)
        {
            int offset = length;
            for (int i = _layers.Length - 1; i >= 0; --i)
            {
                ILayer layer = _layers[i];
                ILayer nextLayer = i == _layers.Length - 1 ? null : _layers[i + 1];
                offset -= layer.Length;
                layer.Finalize(buffer, offset, length - offset - layer.Length, nextLayer);
            }
        }

        private readonly ILayer[] _layers;
        private readonly DataLink _dataLink;
    }
}