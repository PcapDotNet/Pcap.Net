#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class PcapAddress
    {
    public:
        PcapAddress(pcap_addr_t *pcapAddress);

        property SocketAddress^ Address
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Netmask
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Broadcast
        {
            SocketAddress^ get();
        }

        property SocketAddress^ Destination
        {
            SocketAddress^ get();
        }

        virtual System::String^ ToString() override;

    private:
        SocketAddress^ _address;
        SocketAddress^ _netmask;
        SocketAddress^ _broadcast;
        SocketAddress^ _destination;
    };
}}