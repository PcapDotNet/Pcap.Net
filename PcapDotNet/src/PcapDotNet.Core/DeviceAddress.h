#pragma once

#include "SocketAddress.h"
#include "PcapDeclarations.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Representation of an interface address.
    /// </summary>
    public ref class DeviceAddress
    {
    public:
        /// <summary>
        /// The Device Address.
        /// </summary>
        property SocketAddress^ Address
        {
            SocketAddress^ get();
        }

        /// <summary>
        /// if not null, the netmask corresponding to the address in Address. 
        /// </summary>
        property SocketAddress^ Netmask
        {
            SocketAddress^ get();
        }

        /// <summary>
        /// if not null, the broadcast address corresponding to the address in Address; may be null if the interface doesn't support broadcasts.
        /// </summary>
        property SocketAddress^ Broadcast
        {
            SocketAddress^ get();
        }

        /// <summary>
        /// if not null, the destination address corresponding to the address in Address; may be null if the interface isn't a point-to-point interface 
        /// </summary>
        property SocketAddress^ Destination
        {
            SocketAddress^ get();
        }

        virtual System::String^ ToString() override;

    internal:
        DeviceAddress(pcap_addr_t *pcapAddress);

    private:
        static void AppendSocketAddressString(System::Text::StringBuilder^ stringBuilder, SocketAddress^ socketAddress, System::String^ title);

    private:
        SocketAddress^ _address;
        SocketAddress^ _netmask;
        SocketAddress^ _broadcast;
        SocketAddress^ _destination;
    };
}}