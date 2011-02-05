#include "MarshalingServices.h"

#include <Vcclr.h>

using namespace System;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
using namespace PcapDotNet::Base;
using namespace PcapDotNet::Core;

// static 
std::string MarshalingServices::ManagedToUnmanagedString(String^ managedString)
{
    if (String::IsNullOrEmpty(managedString))
        return std::string();

    array<Byte>^ managedBytes = EncodingExtensions::Iso88591->GetBytes(managedString);
    pin_ptr<Byte> pinManagedBytes = &managedBytes[0];
    Byte* unmanagedBytes = pinManagedBytes;
    std::string unmanagedString = std::string(reinterpret_cast<const char*>(unmanagedBytes));
    return unmanagedString;
}

// static
std::wstring MarshalingServices::ManagedToUnmanagedWideString(String^ managedString)
{
    if (String::IsNullOrEmpty(managedString))
        return std::wstring();

    pin_ptr<const wchar_t> managedChars = PtrToStringChars(managedString);
    std::wstring unmanagedString = std::wstring(managedChars);
    return unmanagedString;
}

// static 
array<Byte>^ MarshalingServices::UnamangedToManagedByteArray(const unsigned char* unmanagedByteArray, int offset, int count)
{
    array<Byte>^ managedBytes = gcnew array<Byte>(count);
    Marshal::Copy((IntPtr)const_cast<unsigned char*>(unmanagedByteArray), managedBytes, offset, count);
    return managedBytes;
}
