#include "PcapDataLink.h"

#include <string>

#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace PcapDotNet::Core;

PcapDataLink::PcapDataLink(int value)
{
    _value = value;
}

PcapDataLink::PcapDataLink(String^ name)
{
    std::string unmanagedName = MarshalingServices::ManagedToUnmanagedString(name);
    int value = pcap_datalink_name_to_val(unmanagedName.c_str());
    if (value == -1)
        throw gcnew ArgumentException("Invalid datalink name " + name, "name");

    _value = value;
}

int PcapDataLink::Value::get()
{
    return _value;
}

String^ PcapDataLink::Name::get()
{
    const char* name = pcap_datalink_val_to_name(Value);
    if (name == NULL)
        throw gcnew ArgumentException("datalink " + Value.ToString() + " has no name", "Value");

    return gcnew String(name);
}

String^ PcapDataLink::Description::get()
{
    const char* description = pcap_datalink_val_to_description(Value);
    if (description == NULL)
        throw gcnew ArgumentException("datalink " + Value.ToString() + " has no description", "Value");

    return gcnew String(description);
}


String^ PcapDataLink::ToString()
{
    return Name;
}
