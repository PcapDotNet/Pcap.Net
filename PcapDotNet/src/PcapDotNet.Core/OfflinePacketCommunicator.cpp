#include "OfflinePacketCommunicator.h"

#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace System::Globalization;
using namespace PcapDotNet::Core;

PacketTotalStatistics^ OfflinePacketCommunicator::TotalStatistics::get()
{
    throw gcnew InvalidOperationException("Can't get " + PacketTotalStatistics::typeid->Name + " for offline devices");
}

void OfflinePacketCommunicator::Transmit(PacketSendBuffer^, bool)
{
    throw gcnew InvalidOperationException("Can't transmit queue to an offline device");
}

OfflinePacketCommunicator::~OfflinePacketCommunicator()
{
    if (_file != NULL)
        CloseFile(_file);
}

OfflinePacketCommunicator::OfflinePacketCommunicator(String^ filename)
: PacketCommunicator(OpenFile(filename), nullptr)
{
}

// Private

pcap_t* OfflinePacketCommunicator::OpenFile(String^ fileName)
{
    std::wstring unamangedFilename = MarshalingServices::ManagedToUnmanagedWideString(fileName);
    FILE* file = _wfopen(unamangedFilename.c_str(), L"rb");
	if (file == NULL)
		throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Failed opening file {0}.", fileName));

    char errorBuffer[PCAP_ERRBUF_SIZE];
    pcap_t *pcapDescriptor = pcap_fopen_offline(file, errorBuffer);
    if (pcapDescriptor == NULL)
    {
        CloseFile(file);
        throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Failed opening file {0}. Error: {1}", fileName, gcnew String(errorBuffer)));
    }

    _file = file;

    return pcapDescriptor;
}

// static
void OfflinePacketCommunicator::CloseFile(FILE* file)
{
    int result = fclose(file);
    if (result != 0)
        throw gcnew InvalidOperationException("Failed closing file.");
}