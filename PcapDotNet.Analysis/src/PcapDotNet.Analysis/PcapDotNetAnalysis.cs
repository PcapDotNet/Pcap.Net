using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapDotNet.Analysis
{
    /// <summary>
    /// Used to send PreEmptive Runtime Intelligence Data.
    /// </summary>
    public static class PcapDotNetAnalysis
    {
        /// <summary>
        /// Initializes the Runtime Intelligence and make sures it will be closed on ProcessExit.
        /// </summary>
        public static void Initialize()
        {
            if (!_setup)
            {
                _setup = true;
                Setup();
                AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            }
        }

        /// <summary>
        /// Counts a packet sent event.
        /// </summary>
        [PreEmptive.Attributes.Feature("Sent Packet")]
        public static void PacketSent()
        {
        }

        /// <summary>
        /// Counts a packet sent events according to the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count">The number of packet sent event to count.</param>
        public static void PacketSent(int count)
        {
            for (int i = 0; i != count; ++i)
                PacketSent();
        }

        /// <summary>
        /// Counts a packet received event.
        /// </summary>
        [PreEmptive.Attributes.Feature("Received Packet")]
        public static void PacketReceived()
        {
        }

        /// <summary>
        /// Counts a packet received events according to the given <paramref name="count"/>.
        /// </summary>
        /// <param name="count">The number of packet sent event to count.</param>
        public static void PacketReceived(int count)
        {
            for (int i = 0; i != count; ++i)
                PacketReceived();
        }

        [PreEmptive.Attributes.Setup(CustomEndpoint = "so-s.info/PreEmptive.Web.Services.Messaging/MessagingServiceV2.asmx")]
        private static void Setup()
        {
        }

        [PreEmptive.Attributes.Teardown]
        private static void ProcessExit(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Choose whether to send PreEmptive Runtime Intelligence Data or not.
        /// </summary>
        public static bool OptIn { get; set; }

        private static bool _setup;
    }
}
