#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class IpV4SocketAddress : SocketAddress
    {
	public:
		property Packets::IpV4Address Address
        {
            Packets::IpV4Address get();
        }

        property System::String^ AddressString
        {
            System::String^ get();
        }

        virtual System::String^ ToString() override;

	internal:
        IpV4SocketAddress(sockaddr *address);

    private:
		Packets::IpV4Address _address;
    };
}}