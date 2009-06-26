#include "SocketAddress.h"

using namespace System;
using namespace PcapDotNet;

SocketAddress::SocketAddress(unsigned short family)
{
    _family = safe_cast<SocketAddressFamily>(family);
}

SocketAddressFamily^ SocketAddress::Family::get()
{
    return _family;
}

String^ SocketAddress::ToString()
{
    return Family->ToString();
}
