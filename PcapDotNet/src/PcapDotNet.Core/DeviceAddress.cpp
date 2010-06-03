#include "DeviceAddress.h"
#include "IpV4SocketAddress.h"
#include "IpV6SocketAddress.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace PcapDotNet::Core;

SocketAddress^ DeviceAddress::Address::get()
{
    return _address;
}

SocketAddress^ DeviceAddress::Netmask::get()
{
    return _netmask;
}

SocketAddress^ DeviceAddress::Broadcast::get()
{
    return _broadcast;
}

SocketAddress^ DeviceAddress::Destination::get()
{
    return _destination;
}

String^ DeviceAddress::ToString()
{
    StringBuilder^ result = gcnew StringBuilder();

    AppendSocketAddressString(result, Address, "Address");
    AppendSocketAddressString(result, Netmask, "Netmask");
    AppendSocketAddressString(result, Broadcast, "Broadcast");
    AppendSocketAddressString(result, Destination, "Destination");

    return result->ToString();
}

// Internal

DeviceAddress::DeviceAddress(pcap_addr_t* pcapAddress)
{
    SocketAddressFamily family = safe_cast<SocketAddressFamily>(pcapAddress->addr->sa_family);

    switch (family)
    {
    case SocketAddressFamily::Internet:
        if (pcapAddress->addr)
            _address = gcnew IpV4SocketAddress(pcapAddress->addr);
        if (pcapAddress->netmask)
            _netmask = gcnew IpV4SocketAddress(pcapAddress->netmask);
        if (pcapAddress->broadaddr)
            _broadcast = gcnew IpV4SocketAddress(pcapAddress->broadaddr);
        if (pcapAddress->dstaddr)
            _destination = gcnew IpV4SocketAddress(pcapAddress->dstaddr);
        break;

	case SocketAddressFamily::Internet6:
        if (pcapAddress->addr)
            _address = gcnew IpV6SocketAddress(pcapAddress->addr);
        if (pcapAddress->netmask)
            _netmask = gcnew IpV6SocketAddress(pcapAddress->netmask);
        if (pcapAddress->broadaddr)
            _broadcast = gcnew IpV6SocketAddress(pcapAddress->broadaddr);
        if (pcapAddress->dstaddr)
            _destination = gcnew IpV6SocketAddress(pcapAddress->dstaddr);
		break;

    default:
        throw gcnew NotImplementedException(gcnew String("Device of family ") + family.ToString() + gcnew String(" is unsupported"));
    }
}


// Private

// static
void DeviceAddress::AppendSocketAddressString(StringBuilder^ stringBuilder, SocketAddress^ socketAddress, String^ title)
{
    if (socketAddress != nullptr)
    {
        if (stringBuilder->Length != 0)
            stringBuilder->Append(" ");
        stringBuilder->Append(title);
        stringBuilder->Append(": ");
        stringBuilder->Append(socketAddress);
    }
}