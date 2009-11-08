#include "IpV6SocketAddress.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace System::Net;
using namespace PcapDotNet::Core;
using namespace PcapDotNet::Packets::IpV6;
using namespace PcapDotNet::Base;

IpV6Address IpV6SocketAddress::Address::get()
{
    return _address;
}

String^ IpV6SocketAddress::ToString()
{
    StringBuilder^ result = gcnew StringBuilder();
    result->Append(SocketAddress::ToString());
    result->Append(" ");
    result->Append(Address);
    return result->ToString();
}

// Internal

IpV6SocketAddress::IpV6SocketAddress(sockaddr *address)
: SocketAddress(address->sa_family)
{
	sockaddr_in6* ipV6Address = (struct sockaddr_in6 *)address;
	unsigned long *value = reinterpret_cast<unsigned long*>(ipV6Address->sin6_addr.u.Byte);
	_address = IpV6Address(UInt128(*value, *(value+1)));
}
