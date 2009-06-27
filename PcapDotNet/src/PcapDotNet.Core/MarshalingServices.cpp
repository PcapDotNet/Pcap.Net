#include "MarshalingServices.h"

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
using namespace PcapDotNet::Core;

// static 
std::string MarshalingServices::ManagedToUnmanagedString(System::String^ managedString)
{
    // Marshal the managed string to unmanaged memory.
    array<Byte>^ managedBytes = Encoding::ASCII->GetBytes(managedString);
    pin_ptr<Byte> pinManagedBytes = &managedBytes[0];
    Byte* unmanagedBytes = pinManagedBytes;
    std::string unmanagedString = std::string((char*)unmanagedBytes);
    return unmanagedString;
}

// static 
array<Byte>^ MarshalingServices::UnamangedToManagedByteArray(const unsigned char* unmanagedByteArray, int offset, int count)
{
    array<Byte>^ managedBytes = gcnew array<Byte>(count);
    Marshal::Copy((IntPtr)const_cast<unsigned char*>(unmanagedByteArray), managedBytes, offset, count);
    return managedBytes;
}
