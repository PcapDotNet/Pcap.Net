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
    if (value != -1)
    {
        _value = value;
        return;
    }

    if (name == "PPP_WITH_DIR")
    {
        _value = 204;
        return;
    }

    throw gcnew ArgumentException("Invalid datalink name " + name, "name");
}

DataLinkKind PcapDataLink::Kind::get()
{
    switch (Value)
    {
    case DLT_EN10MB:
        return DataLinkKind::Ethernet;

    case DLT_RAW:
        return DataLinkKind::IpV4;

    case DLT_DOCSIS:
		return DataLinkKind::Docsis;

    case DLT_PPP_WITH_DIR:
        return DataLinkKind::PppWithDirection;

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
    if (name != NULL) 
        return gcnew String(name);

    switch (Value) 
    {
    case DLT_PPP_WITH_DIR: 
        return "PPP_WITH_DIR";

    default:
        throw gcnew InvalidOperationException(PcapDataLink::typeid->Name + " " + Value.ToString(CultureInfo::InvariantCulture) + " has no name");
    }
}

String^ PcapDataLink::Description::get()
{
    const char* description = pcap_datalink_val_to_description(Value);
    if (description != NULL)
        return gcnew String(description);

    switch (Value) 
    {
    case DLT_PPP_WITH_DIR: 
        return "PPP with Directional Info";

    default:
        throw gcnew InvalidOperationException(PcapDataLink::typeid->Name + " " + Value.ToString(CultureInfo::InvariantCulture) + " has no description");
    }
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
        return DLT_EN10MB;

    case DataLinkKind::IpV4:
        return DLT_RAW;

	case DataLinkKind::Docsis:
        return DLT_DOCSIS;

    case DataLinkKind::PppWithDirection:
        return DLT_PPP_WITH_DIR;

	default:
        throw gcnew NotSupportedException(PcapDataLink::typeid->Name + " kind " + kind.ToString() + " is unsupported");
    }
}
