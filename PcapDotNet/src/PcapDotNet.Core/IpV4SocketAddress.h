#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    public ref class IpV4SocketAddress : SocketAddress
    {
    public:
        IpV4SocketAddress(sockaddr *address);

        property unsigned int Address
        {
            unsigned int get();
        }

        property System::String^ AddressString
        {
            System::String^ get();
        }

        virtual System::String^ ToString() override;

    private:
        unsigned int _address;
    };
}}