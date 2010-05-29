using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapDotNet.Analysis
{
    public static class PcapDotNetAnalysis
    {
        public static void Initialize()
        {
            if (!_setup)
            {
                _setup = true;
                Setup();
                AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            }
        }

        [PreEmptive.Attributes.Feature("Sent Packet")]
        public static void PacketSent()
        {
        }

        public static void PacketSent(int count)
        {
            for (int i = 0; i != count; ++i)
                PacketSent();
        }

        [PreEmptive.Attributes.Feature("Received Packet")]
        public static void PacketReceived()
        {
        }

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

        public static bool OptIn { get; set; }

        private static bool _setup;
    }
}
