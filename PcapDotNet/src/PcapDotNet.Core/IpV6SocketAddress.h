#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// An internet protocol version 6 address for a device.
    /// </summary>
    public ref class IpV6SocketAddress : SocketAddress
    {
	public:
        /// <summary>
        /// The ip version 6 address.
        /// </summary>
		property Packets::IpV6::IpV6Address Address
        {
            Packets::IpV6::IpV6Address get();
        }

        virtual System::String^ ToString() override;

	internal:
        IpV6SocketAddress(sockaddr *address);

    private:
		Packets::IpV6::IpV6Address _address;
    };
}}