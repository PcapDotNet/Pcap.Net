namespace PcapDotNet.Packets.Arp
{
    public enum ArpHardwareType : ushort
    {
        /// <summary>
        /// Ethernet (10Mb)
        /// </summary>
        Ethernet = 1,

        /// <summary>
        /// Experimental Ethernet (3Mb)
        /// </summary>
        ExperimentalEthernet = 2,

        /// <summary>
        /// Amateur Radio AX.25
        /// </summary>
        AmateurRadioAx25 = 3,

        /// <summary>
        /// Proteon ProNET Token Ring
        /// </summary>
        ProteonProNetTokenRing = 4,

        /// <summary>
        /// Chaos
        /// </summary>
        Chaos = 5,

        /// <summary>
        /// IEEE 802 Networks
        /// </summary>
        Ieee802Networks = 6,

        /// <summary>
        /// ARCNET
        /// </summary>
        Arcnet = 7,

        /// <summary>
        /// Hyperchannel
        /// </summary>
        Hyperchannel = 8,

        /// <summary>
        /// Lanstar
        /// </summary>
        Lanstar = 9,

        /// <summary>
        /// Autonet Short Address
        /// </summary>
        AutonetShortAddress = 10,

        /// <summary>
        /// LocalTalk
        /// </summary>
        LocalTalk = 11,

        /// <summary>
        /// LocalNet (IBM PCNet or SYTEK LocalNET)
        /// </summary>
        LocalNet = 12,

        /// <summary>
        /// Ultra link
        /// </summary>
        UltraLink = 13,

        /// <summary>
        /// SMDS
        /// </summary>
        Smds = 14,

        /// <summary>
        /// Frame Relay
        /// </summary>
        FrameRelay = 15,

        /// <summary>
        /// Asynchronous Transmission Mode (ATM)
        /// </summary>
        AsynchronousTransmissionMode16 = 16,

        /// <summary>
        /// HDLC
        /// </summary>
        Hdlc = 17,

        /// <summary>
        /// Fibre Channel
        /// </summary>
        FibreChannel = 18,

        /// <summary>
        /// Asynchronous Transmission Mode (ATM)
        /// </summary>
        AsynchronousTransmissionMode19 = 19,

        /// <summary>
        /// Serial Line
        /// </summary>
        SerialLine = 20,

        /// <summary>
        /// Asynchronous Transmission Mode (ATM)
        /// </summary>
        AsynchronousTransmissionMode21 = 21,

        /// <summary>
        /// MIL-STD-188-220
        /// </summary>
        MilStd188_220 = 22,

        /// <summary>
        /// Metricom
        /// </summary>
        Metricom = 23,

        /// <summary>
        /// IEEE 1394.1995
        /// </summary>
        Ieee1394_1995 = 24,

        /// <summary>
        /// MAPOS
        /// </summary>
        Mapos = 25,

        /// <summary>
        /// Twinaxial
        /// </summary>
        Twinaxial = 26,

        /// <summary>
        /// EUI-64
        /// </summary>
        Eui64 = 27,

        /// <summary>
        /// HIPARP
        /// </summary>
        Hiparp = 28,

        /// <summary>
        /// IP and ARP over ISO 7816-3
        /// </summary>
        IpAndArpOverIso7816_3 = 29,

        /// <summary>
        /// ARPSec
        /// </summary>
        ArpSec = 30,

        /// <summary>
        /// IPsec tunnel
        /// </summary>
        IpSecTunnel = 31,

        /// <summary>
        /// InfiniBand (TM)
        /// </summary>
        InfiniBand = 32,

        /// <summary>
        /// TIA-102 Project 25 Common Air Interface (CAI)
        /// </summary>
        Tia102Project25CommonAirInterface = 33,

        /// <summary>
        /// Wiegand Interface
        /// </summary>
        WiegandInterface = 34,

        /// <summary>
        /// Pure IP
        /// </summary>
        PureIp = 35,

        /// <summary>
        /// HW_EXP1
        /// </summary>
        Experimental1 = 36,

        /// <summary>
        /// HW_EXP2
        /// </summary>
        Experimental2 = 256,
    }
}