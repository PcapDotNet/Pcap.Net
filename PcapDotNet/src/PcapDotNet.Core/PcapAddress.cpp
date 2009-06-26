#include "PcapAddress.h"
#include "IpV4SocketAddress.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace PcapDotNet;

PcapAddress::PcapAddress(pcap_addr_t* pcapAddress)
{
    SocketAddressFamily family = safe_cast<SocketAddressFamily>(pcapAddress->addr->sa_family);

    switch (family)
    {
    case SocketAddressFamily::INET:
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

SocketAddress^ PcapAddress::Address::get()
{
    return _address;
}

SocketAddress^ PcapAddress::Netmask::get()
{
    return _netmask;
}

SocketAddress^ PcapAddress::Broadcast::get()
{
    return _broadcast;
}

SocketAddress^ PcapAddress::Destination::get()
{
    return _destination;
}

String^ PcapAddress::ToString()
{
    StringBuilder^ result = gcnew StringBuilder();

    if (Address != nullptr)
    {
        result->Append("Address: ");
        result->Append(Address->ToString());
    }

    if (Netmask != nullptr)
    {
        if (result->Length != 0)
            result->Append(" ");
        result->Append("Netmask: ");
        result->Append(Netmask->ToString());
    }

    if (Broadcast != nullptr)
    {
        if (result->Length != 0)
            result->Append(" ");
        result->Append("Broadcast: ");
        result->Append(Broadcast->ToString());
    }

    if (Destination != nullptr)
    {
        if (result->Length != 0)
            result->Append(" ");
        result->Append("Destination: ");
        result->Append(Destination->ToString());
    }

    return result->ToString();
}