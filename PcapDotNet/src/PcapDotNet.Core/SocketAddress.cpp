#include "SocketAddress.h"

using namespace System;
using namespace PcapDotNet::Core;

SocketAddressFamily^ SocketAddress::Family::get()
{
    return _family;
}

String^ SocketAddress::ToString()
{
    return Family->ToString();
}

SocketAddress::SocketAddress(unsigned short family)
{
    _family = safe_cast<SocketAddressFamily>(family);
}
