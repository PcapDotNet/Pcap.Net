#pragma once

namespace PcapDotNet { namespace Core 
{
	/// <summary>
	/// Flags to use when openning a device to send and receive packets.
	/// </summary>
    [System::Flags]
    public enum class PacketDeviceOpenAttributes : System::Int32
    {
		/// <summary>
		/// No flags.
		/// </summary>
        None                    = 0,

		/// <summary>
		/// Defines if the adapter has to go in promiscuous mode.
		/// Note that even if this parameter is false, the interface could well be in promiscuous mode for some other reason
		/// (for example because another capture process with promiscuous mode enabled is currently using that interface).
		/// On on Linux systems with 2.2 or later kernels (that have the "any" device), this flag does not work on the "any" device;
		/// if an argument of "any" is supplied, the 'promisc' flag is ignored. 
		/// </summary>
        Promiscuous             = 1,

		/// <summary>
		/// Defines if the data trasfer (in case of a remote capture) has to be done with UDP protocol.
		/// Use this flag if you want a UDP data connection, don't use it if you want a TCP data connection; control connection is always TCP-based.
		/// A UDP connection is much lighter, but it does not guarantee that all the captured packets arrive to the client workstation.
		/// Moreover, it could be harmful in case of network congestion.
		/// This flag is meaningless if the source is not a remote interface. In that case, it is simply ignored. 
		/// </summary>
        DataTransferUdpRemote   = 2,

		/// <summary>
		/// Defines if the remote probe will capture its own generated traffic.
		/// In case the remote probe uses the same interface to capture traffic and to send data back to the caller,
		/// the captured traffic includes the RPCAP traffic as well.
		/// If this flag is turned on, the RPCAP traffic is excluded from the capture,
		/// so that the trace returned back to the collector does not include this traffic. 
		/// </summary>
        NoCaptureRemote         = 4,

		/// <summary>
		/// Defines if the local adapter will capture its own generated traffic.
		/// This flag tells the underlying capture driver to drop the packets that were sent by itself.
		/// This is useful when building applications like bridges, that should ignore the traffic they just sent. 
		/// Note that this flag applies to the specific PacketCommunicator opened with it only.
		/// </summary>
        NoCaptureLocal          = 8,

		/// <summary>
		/// This flag configures the adapter for maximum responsiveness.
		/// In presence of a large value for nbytes, WinPcap waits for the arrival of several packets before copying the data to the user.
		/// This guarantees a low number of system calls, i.e. lower processor usage, i.e. better performance, which is good for applications like sniffers.
		/// If the user sets this flag, the capture driver will copy the packets as soon as the application is ready to receive them.
		/// This is suggested for real time applications (like, for example, a bridge) that need the best responsiveness. 
		/// </summary>
        MaximumResponsiveness   = 16
    };
}}