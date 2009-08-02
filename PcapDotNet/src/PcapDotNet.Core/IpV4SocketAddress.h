#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// An internet protocol version 4 address for a device.
    /// </summary>
    public ref class IpV4SocketAddress : SocketAddress
    {
	public:
        /// <summary>
        /// The ip version 4 address.
        /// </summary>
		property Packets::IpV4::IpV4Address Address
        {
            Packets::IpV4::IpV4Address get();
        }

        virtual System::String^ ToString() override;

	internal:
        IpV4SocketAddress(sockaddr *address);

    private:
		Packets::IpV4::IpV4Address _address;
    };
}}