using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcapDotNet.Packets;

namespace PcapDotNet.Core.Test
{
    internal class PacketHandler
    {
        public PacketHandler(Packet expectedPacket, DateTime expectedMinTimestamp, DateTime expectedMaxTimestamp, 
            PacketCommunicator communicator, int numPacketsToBreakLoop)
        {
            _expectedPacket = expectedPacket;
            _expectedMinTimestamp = expectedMinTimestamp;
            _expectedMaxTimestamp = expectedMaxTimestamp;
            _communicator = communicator;
            _numPacketsToBreakLoop = numPacketsToBreakLoop;
        }

        public PacketHandler(Packet expectedPacket, double expectedSecondsBefore, double expectedSecondsAfter, PacketCommunicator communicator, int numPacketsToBreakLoop)
            : this(expectedPacket, expectedPacket.Timestamp.AddSeconds(-expectedSecondsBefore), expectedPacket.Timestamp.AddSeconds(expectedSecondsAfter), communicator, numPacketsToBreakLoop)
        {
        }

        public PacketHandler(Packet expectedPacklet, PacketCommunicator communicator, int numPacketsToBreakLoop)
            : this(expectedPacklet, DateTime.MinValue, DateTime.MaxValue, communicator, numPacketsToBreakLoop)
        {
        }

        public void Handle(Packet packet)
        {
            Assert.AreEqual(_expectedPacket, packet);
            MoreAssert.IsInRange(_expectedMinTimestamp, _expectedMaxTimestamp, packet.Timestamp);
            ++_numPacketsHandled;
            if (NumPacketsHandled >= _numPacketsToBreakLoop)
                _communicator.Break();
        }

        public int NumPacketsHandled
        {
            get { return _numPacketsHandled; }
        }

        private readonly Packet _expectedPacket;
        private readonly DateTime _expectedMinTimestamp;
        private readonly DateTime _expectedMaxTimestamp;
        private readonly PacketCommunicator _communicator;
        private readonly int _numPacketsToBreakLoop = int.MaxValue;
        private int _numPacketsHandled; // = 0;
    }
}