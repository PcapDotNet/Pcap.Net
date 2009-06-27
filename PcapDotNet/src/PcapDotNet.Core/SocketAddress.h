#pragma once

#include "SocketAddressFamily.h"

namespace PcapDotNet { namespace Core 
{
    public ref class SocketAddress
    {
    public:
        SocketAddress(unsigned short family);

        property SocketAddressFamily^ Family
        {
            SocketAddressFamily^ get();
        }

        virtual System::String^ ToString() override;

    private:
        SocketAddressFamily^ _family;
    };
}}