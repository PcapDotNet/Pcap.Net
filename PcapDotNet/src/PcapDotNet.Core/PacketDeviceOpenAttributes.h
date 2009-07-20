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
		/// </summary>
        Promiscuous             = 1,

		/// <summary>
		/// Defines if the data trasfer (in case of a remote capture) has to be done with UDP protocol.
		/// </summary>
        DataTransferUdpRemote   = 2,

		/// <summary>
		/// Defines if the remote probe will capture its own generated traffic.
		/// </summary>
        NoCaptureRemote         = 4,

		/// <summary>
		/// Defines if the local adapter will capture its own generated traffic.
		/// </summary>
        NoCaptureLocal          = 8,

		/// <summary>
		/// This flag configures the adapter for maximum responsiveness.     
		/// </summary>
        MaximumResponsiveness   = 16
    };
}}