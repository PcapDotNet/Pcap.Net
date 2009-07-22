#include "IpV4SocketAddress.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace PcapDotNet::Core;
using namespace Packets;

IpV4Address IpV4SocketAddress::Address::get()
{
    return _address;
}

String^ IpV4SocketAddress::AddressString::get()
{
	return Address.ToString();
}

String^ IpV4SocketAddress::ToString()
{
    StringBuilder^ result = gcnew StringBuilder();
    result->Append(SocketAddress::ToString());
    result->Append(" ");
    result->Append(AddressString);
    return result->ToString();
}

// Internal

IpV4SocketAddress::IpV4SocketAddress(sockaddr *address)
: SocketAddress(address->sa_family)
{
    sockaddr_in* ipV4Address = (struct sockaddr_in *)address;
	_address = IpV4Address::FromReversedEndianity(ipV4Address->sin_addr.S_un.S_addr);
}
