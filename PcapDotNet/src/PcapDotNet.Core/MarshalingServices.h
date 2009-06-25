#pragma once

#include <string>

namespace PcapDotNet 
{
    public ref class MarshalingServices
    {
    public:
        static std::string ManagedToUnmanagedString(System::String^ managedString);

        static array<System::Byte>^ UnamangedToManagedByteArray(const unsigned char* unmanagedByteArray, int offset, int count);
    };
}
