#include "PcapDataLink.h"

#include <string>

#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace System::Globalization;
using namespace PcapDotNet::Packets;
using namespace PcapDotNet::Core;

PcapDataLink::PcapDataLink(DataLinkKind kind)
{
    _value = KindToValue(kind);
}

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

DataLinkKind PcapDataLink::Kind::get()
{
    switch (Value)
    {
    case 1:
        return DataLinkKind::Ethernet;

    case 12:
        return DataLinkKind::IpV4;

    case 143:
		return DataLinkKind::Docsis;

	default:
        throw gcnew NotSupportedException(PcapDataLink::typeid->Name + " " + Value.ToString(CultureInfo::InvariantCulture) + " - " + ToString() + " is unsupported");
    }
}


int PcapDataLink::Value::get()
{
    return _value;
}

String^ PcapDataLink::Name::get()
{
    const char* name = pcap_datalink_val_to_name(Value);
    if (name == NULL)
        throw gcnew InvalidOperationException(PcapDataLink::typeid->Name + " " + Value.ToString(CultureInfo::InvariantCulture) + " has no name");

    return gcnew String(name);
}

String^ PcapDataLink::Description::get()
{
    const char* description = pcap_datalink_val_to_description(Value);
    if (description == NULL)
        throw gcnew InvalidOperationException(PcapDataLink::typeid->Name + " " + Value.ToString(CultureInfo::InvariantCulture) + " has no description");

    return gcnew String(description);
}

String^ PcapDataLink::ToString()
{
    return Name + " (" + Description + ")";
}

bool PcapDataLink::Equals(PcapDataLink other)
{
    return _value == other._value;
}

bool PcapDataLink::Equals(System::Object^ obj)
{
    PcapDataLink^ other = dynamic_cast<PcapDataLink^>(obj);
    if (other == nullptr)
        return false;

    return Equals((PcapDataLink)other);
}

int PcapDataLink::GetHashCode()
{
    return Value.GetHashCode();
}

// static
bool PcapDataLink::operator ==(PcapDataLink dataLink1, PcapDataLink dataLink2)
{
    return dataLink1.Equals(dataLink2);
}

// static
bool PcapDataLink::operator !=(PcapDataLink dataLink1, PcapDataLink dataLink2)
{
    return !dataLink1.Equals(dataLink2);
}

// private

int PcapDataLink::KindToValue(DataLinkKind kind)
{
    switch (kind)
    {
    case DataLinkKind::Ethernet:
        return 1;

    case DataLinkKind::IpV4:
        return 12;

	case DataLinkKind::Docsis:
        return 143;

	default:
        throw gcnew NotSupportedException(PcapDataLink::typeid->Name + " kind " + kind.ToString() + " is unsupported");
    }
}
