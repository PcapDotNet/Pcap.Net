#include "DeviceAddress.h"
#include "IpV4SocketAddress.h"
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

    default:
        throw gcnew NotImplementedException(gcnew String("Device of family ") + family.ToString() + gcnew String(" is unsupported"));
    //            case SocketAddressFamily::INET6:
        //break;


    /*                case AF_INET:
        printf("\tAddress Family Name: AF_INET\n");
        if (a->addr)
          printf("\tAddress: %s\n",iptos(((struct sockaddr_in *)a->addr)->sin_addr.s_addr));
        if (a->netmask)
          printf("\tNetmask: %s\n",iptos(((struct sockaddr_in *)a->netmask)->sin_addr.s_addr));
        if (a->broadaddr)
          printf("\tBroadcast Address: %s\n",iptos(((struct sockaddr_in *)a->broadaddr)->sin_addr.s_addr));
        if (a->dstaddr)
          printf("\tDestination Address: %s\n",iptos(((struct sockaddr_in *)a->dstaddr)->sin_addr.s_addr));
        break;

      case AF_INET6:
        printf("\tAddress Family Name: AF_INET6\n");
        if (a->addr)
          printf("\tAddress: %s\n", ip6tos(a->addr, ip6str, sizeof(ip6str)));
       break;

      default:
        printf("\tAddress Family Name: Unknown\n");
        break;*/
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