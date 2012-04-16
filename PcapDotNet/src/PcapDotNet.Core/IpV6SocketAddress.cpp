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
    Byte* byteValue = ipV6Address->sin6_addr.u.Byte;
    UInt128 value128 = BitSequence::Merge(byteValue[0], byteValue[1], byteValue[2], byteValue[3],
                                          byteValue[4], byteValue[5], byteValue[6], byteValue[7],
                                          byteValue[8], byteValue[9], byteValue[10], byteValue[11],
                                          byteValue[12], byteValue[13], byteValue[14], byteValue[15]);
	_address = IpV6Address(value128);
}
