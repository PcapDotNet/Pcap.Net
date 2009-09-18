using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PcapDotNet.Packets;

namespace PcapDotNet.Core.Extensions
{
    /// <summary>
    /// Different extension methods for PacketCommunicator class.
    /// <seealso cref="PacketCommunicator"/>
    /// </summary>
    public static class MorePacketCommunicator
    {
        /// <summary>
        /// Collect a group of packets.
        /// Similar to ReceivePackets() except instead of calling a callback the packets are returned as an IEnumerable.
        /// <seealso cref="PacketCommunicator.ReceivePackets"/>
        /// <seealso cref="PacketCommunicator.ReceiveSomePackets"/>
        /// </summary>
        /// <param name="communicator">The PacketCommunicator to work on</param>
        /// <param name="count">Number of packets to process. A negative count causes ReceivePackets() to loop until the IEnumerable break (or until an error occurs).</param>
        /// <returns>An IEnumerable of Packets to process.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the mode is not Capture or an error occurred.</exception>
        /// <remarks>
        ///   <para>Only the first bytes of data from the packet might be in the received packet (which won't necessarily be the entire packet; to capture the entire packet, you will have to provide a value for snapshortLength in your call to PacketDevice.Open() that is sufficiently large to get all of the packet's data - a value of 65536 should be sufficient on most if not all networks).</para>
        ///   <para>If a break is called on the returned Enumerable before the number of packets asked for received, the packet that was handled while breaking the enumerable may not be the last packet read. More packets might be read. This is because this method actually loops over calls to ReceiveSomePackets()</para>
        /// </remarks>
        public static IEnumerable<Packet> ReceivePackets(this PacketCommunicator communicator, int count)
        {
            List<Packet> packets = new List<Packet>(Math.Min(Math.Max(0, count), 128));

            while (count != 0)
            {
                packets.Clear();

                int countGot;
                PacketCommunicatorReceiveResult result = communicator.ReceiveSomePackets(out countGot, count, packets.Add);
                if (count > 0)
                    count -= countGot;
                foreach (Packet packet in packets)
                    yield return packet;

                if (result != PacketCommunicatorReceiveResult.Ok)
                    break;
            }
        }

        /// <summary>
        /// Collect a group of packets.
        /// Similar to ReceivePackets() except instead of calling a callback the packets are returned as an IEnumerable.
        /// Loops until the IEnumerable break (or until an error occurs).
        /// <seealso cref="PacketCommunicator.ReceivePackets"/>
        /// <seealso cref="PacketCommunicator.ReceiveSomePackets"/>
        /// </summary>
        /// <param name="communicator">The PacketCommunicator to work on</param>
        /// <returns>An IEnumerable of Packets to process.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the mode is not Capture or an error occurred.</exception>
        /// <remarks>
        ///   <para>Only the first bytes of data from the packet might be in the received packet (which won't necessarily be the entire packet; to capture the entire packet, you will have to provide a value for snapshortLength in your call to PacketDevice.Open() that is sufficiently large to get all of the packet's data - a value of 65536 should be sufficient on most if not all networks).</para>
        ///   <para>If a break is called on the returned Enumerable, the packet that was handled while breaking the enumerable may not be the last packet read. More packets might be read. This is because this method actually loops over calls to ReceiveSomePackets()</para>
        /// </remarks>
        public static IEnumerable<Packet> ReceivePackets(this PacketCommunicator communicator)
        {
            return communicator.ReceivePackets(-1);
        }
    }
}

