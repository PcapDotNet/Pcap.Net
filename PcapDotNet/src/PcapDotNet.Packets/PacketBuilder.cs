using System;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// This class is used to build different packets.
    /// Packets are built from layers.
    /// This class can be used using static Build methods by giving the Packet's timestamp and layers.
    /// This class can be instantiated with different layers and then use the Build method by only giving the Packet's timestamp.
    /// If a layer that an instance of this class holds is modified, the PacketBuilder instance will create different packets.
    /// <example>This sample shows how to create ICMP Echo Request packets to different servers with different IP ID and ICMP sequence numbers and identifiers.
    /// <code>
    ///   EthernetLayer ethernetLayer = new EthernetLayer
    ///                                     {
    ///                                         Source = new MacAddress("00:01:02:03:04:05"),
    ///                                         Destination = new MacAddress("A0:A1:A2:A3:A4:A5")
    ///                                     };
    ///
    ///   IpV4Layer ipV4Layer = new IpV4Layer
    ///                             {
    ///                                 Source = new IpV4Address("1.2.3.4"),
    ///                                 Ttl = 128,
    ///                             };
    ///
    ///   IcmpEchoLayer icmpLayer = new IcmpEchoLayer();
    ///
    ///   PacketBuilder builder = new PacketBuilder(ethernetLayer, ipV4Layer, icmpLayer);
    ///
    ///   List&lt;Packet&gt; packets = new List&lt;Packet&gt;();
    ///
    ///   for (int i = 0; i != 100; ++i)
    ///   {
    ///       ipV4Layer.Destination = new IpV4Address("2.3.4." + i);
    ///       ipV4Layer.Identification = (ushort)i;
    ///       icmpLayer.SequenceNumber = (ushort)i;
    ///       icmpLayer.Identifier = (ushort)i;
    ///
    ///       packets.Add(builder.Build(DateTime.Now));
    ///   }
    /// </code>
    /// </example>
    /// </summary>
    public class PacketBuilder
    {
        /// <summary>
        /// Builds a single packet using the given layers with the given timestamp.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, params ILayer[] layers)
        {
            return new PacketBuilder(layers).Build(timestamp);
        }

        /// <summary>
        /// Builds a single packet using the given layers with the given timestamp.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, IEnumerable<ILayer> layers)
        {
            return new PacketBuilder(layers).Build(timestamp);
        }

        /// <summary>
        /// Creates a PacketBuilder that can build packets according to the given layers and with different timestamps.
        /// The layers' properties can be modified after the builder is built and this will affect the packets built.
        /// </summary>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
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


        /// <summary>
        /// Creates a PacketBuilder that can build packets according to the given layers and with different timestamps.
        /// The layers' properties can be modified after the builder is built and this will affect the packets built.
        /// </summary>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        public PacketBuilder(IEnumerable<ILayer> layers)
            :this(layers.ToArray())
        {
        }

        /// <summary>
        /// Builds a single packet using the builder's layers with the given timestamp.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <returns>A packet built from the builder's layers with the given timestamp.</returns>
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