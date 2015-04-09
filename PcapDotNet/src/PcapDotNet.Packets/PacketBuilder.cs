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
        /// <param name="originalLength">
        /// Length this packet (off wire). 
        /// If the value is less than the actual size, it is ignored and the original length is considered to be equal to the actual size.
        /// </param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, uint originalLength, params ILayer[] layers)
        {
            return new PacketBuilder(layers).Build(timestamp, originalLength);
        }

        /// <summary>
        /// Builds a single packet using the given layers with the given timestamp.
        /// The original length is considered to be the actual size.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, params ILayer[] layers)
        {
            return Build(timestamp, 0, layers);
        }

        /// <summary>
        /// Builds a single packet using the given layers with the given timestamp.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <param name="originalLength">
        /// Length this packet (off wire). 
        /// If the value is less than the actual size, it is ignored and the original length is considered to be equal to the actual size.
        /// </param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, uint originalLength, IEnumerable<ILayer> layers)
        {
            return new PacketBuilder(layers).Build(timestamp, originalLength);
        }

        /// <summary>
        /// Builds a single packet using the given layers with the given timestamp.
        /// The original length is considered to be the actual size.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        /// <returns>A packet built from the given layers with the given timestamp.</returns>
        public static Packet Build(DateTime timestamp, IEnumerable<ILayer> layers)
        {
            return Build(timestamp, 0, layers);
        }

        /// <summary>
        /// Creates a PacketBuilder that can build packets according to the given layers and with different timestamps.
        /// The layers' properties can be modified after the builder is built and this will affect the packets built.
        /// </summary>
        /// <param name="layers">The layers to build the packet accordingly and by their order.</param>
        public PacketBuilder(params ILayer[] layers)
        {
            if (layers == null) 
                throw new ArgumentNullException("layers");

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
        /// <param name="originalLength">
        /// Length this packet (off wire). 
        /// If the value is less than the actual size, it is ignored and the original length is considered to be equal to the actual size.
        /// </param>
        /// <returns>A packet built from the builder's layers with the given timestamp.</returns>
        public Packet Build(DateTime timestamp, uint originalLength)
        {
            int[] layersLength = _layers.Select(layer => layer.Length).ToArray();
            int length = layersLength.Sum();
            byte[] buffer = new byte[length];

            WriteLayers(layersLength, buffer, length);
            FinalizeLayers(buffer, length);

            return new Packet(buffer, timestamp, _dataLink, originalLength);
        }

        /// <summary>
        /// Builds a single packet using the builder's layers with the given timestamp.
        /// The original length is considered to be the actual size.
        /// </summary>
        /// <param name="timestamp">The packet's timestamp.</param>
        /// <returns>A packet built from the builder's layers with the given timestamp.</returns>
        public Packet Build(DateTime timestamp)
        {
            return Build(timestamp, 0);
        }

        private void WriteLayers(int[] layersLength, byte[] buffer, int length)
        {
            int offset = 0;
            for (int i = 0; i != _layers.Length; ++i)
            {
                ILayer layer = _layers[i];
                int layerLength = layersLength[i];
                ILayer previousLayer = i == 0 ? null : _layers[i - 1];
                ILayer nextLayer = i == _layers.Length - 1 ? null : _layers[i + 1];
                layer.Write(buffer, offset, length - offset - layerLength, previousLayer, nextLayer);
                offset += layerLength;
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